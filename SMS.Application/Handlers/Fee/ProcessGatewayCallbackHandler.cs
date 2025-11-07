using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;
using SMS.Application.Dto;

namespace SMS.Application.Handlers.Fee
{
    public class ProcessGatewayCallbackHandler : IRequestHandler<ProcessGatewayCallbackCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPaymentGateway _gateway;

        public ProcessGatewayCallbackHandler(IUnitOfWork uow, IPaymentGateway gateway)
        {
            _uow = uow; _gateway = gateway;
        }

        public async Task<int> Handle(ProcessGatewayCallbackCommand request, CancellationToken cancellationToken)
        {
            var cb = request.Callback;

            var order = await _uow.FeeRepository.GetPaymentOrderByOrderNoAsync(cancellationToken, cb.OrderNo);
            if (order is null) return -1;

            // Verify signature (if provided)
            var verified = await _gateway.VerifyCallbackAsync(new VerifyPaymentContext
            {
                OrderNo = cb.OrderNo,
                PaymentId = cb.PaymentId,
                GatewayOrderId = cb.GatewayOrderId,
                Signature = cb.Signature,
                Amount = cb.Amount,
                Currency = cb.Currency,
                RawPayload = cb.RawPayload
            }, cancellationToken);

            await _uow.FeeRepository.InsertPaymentGatewayEventAsync(cancellationToken, new PaymentGatewayEvent
            {
                OrderId = order.OrderId,
                EventType = "Callback",
                Payload = cb.RawPayload ?? JsonSerializer.Serialize(cb)
            });

            if (!verified)
            {
                await _uow.FeeRepository.UpdatePaymentOrderStatusAsync(cancellationToken, order.OrderId, "Failed", cb.PaymentId, cb.GatewayOrderId, cb.PaymentId);
                return 0;
            }

            // If already processed (has receipt), idempotent
            if (order.ReceiptId.HasValue) return order.ReceiptId.Value;

            if (cb.Status == "Success")
            {
                // Create receipt + ledger credits from ItemsJson
                var items = JsonSerializer.Deserialize<List<FeeReceiptItemDto>>(order.ItemsJson) ?? new();

                var header = new FeeReceipt
                {
                    StudentId = order.StudentId,
                    AcademicYear = order.AcademicYear,
                    TermId = order.TermId,
                    PaymentMode = "OnlineGateway",
                    ReferenceNo = cb.PaymentId,
                    TotalAmount = order.Amount,
                    ReceiptDate = System.DateTime.UtcNow,
                    ReceivedByUserId = null
                };
                var receiptId = await _uow.FeeRepository.CreateFeeReceiptAsync(cancellationToken, header);

                foreach (var item in items)
                {
                    await _uow.FeeRepository.AddFeeReceiptItemAsync(cancellationToken, new FeeReceiptItem
                    {
                        ReceiptId = receiptId,
                        HeadId = item.HeadId,
                        Amount = item.Amount
                    });

                    await _uow.FeeRepository.PostLedgerCreditAsync(cancellationToken, new StudentFeeLedger
                    {
                        StudentId = order.StudentId,
                        AcademicYear = order.AcademicYear,
                        TermId = order.TermId,
                        HeadId = item.HeadId,
                        EntryType = "Credit",
                        Amount = item.Amount,
                        Narration = $"Online Payment #{cb.PaymentId}",
                        ReceiptId = receiptId,
                        EntryDate = System.DateTime.UtcNow.Date
                    });
                }

                await _uow.FeeRepository.MarkPaymentOrderReceiptedAsync(cancellationToken, order.OrderId, receiptId);
                await _uow.FeeRepository.UpdatePaymentOrderStatusAsync(cancellationToken, order.OrderId, "Success", cb.PaymentId, cb.GatewayOrderId, cb.PaymentId);

                return receiptId;
            }
            else
            {
                var status = cb.Status == "Cancelled" ? "Cancelled" : "Failed";
                await _uow.FeeRepository.UpdatePaymentOrderStatusAsync(cancellationToken, order.OrderId, status, cb.PaymentId, cb.GatewayOrderId, cb.PaymentId);
                return 0;
            }
        }
    }
}