using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class InitiateOnlinePaymentHandler : IRequestHandler<InitiateOnlinePaymentCommand, InitiateOnlinePaymentResponseDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPaymentGateway _gateway;

        public InitiateOnlinePaymentHandler(IUnitOfWork uow, IPaymentGateway gateway)
        {
            _uow = uow; _gateway = gateway;
        }

        public async Task<InitiateOnlinePaymentResponseDto> Handle(InitiateOnlinePaymentCommand request, CancellationToken cancellationToken)
        {
            var r = request.Request;
            var amount = r.Items?.Sum(i => i.Amount) ?? 0m;

            var order = new PaymentGatewayOrder
            {
                StudentId = r.StudentId,
                AcademicYear = r.AcademicYear,
                TermId = r.TermId,
                Amount = amount,
                Currency = r.Currency,
                Status = "Initiated",
                GatewayName = _gateway.Name,
                ReturnUrl = r.ReturnUrl,
                CallbackUrl = r.CallbackUrl,
                PaymentMode = "OnlineGateway",
                ItemsJson = JsonSerializer.Serialize(r.Items ?? new())
            };

            var orderId = await _uow.FeeRepository.CreatePaymentOrderAsync(cancellationToken, order);
            //var orderFromDb = await _uow.FeeRepository.GetPaymentOrderByOrderNoAsync(cancellationToken, order.OrderNo);
            var orderFromDb = await _uow.FeeRepository.GetPaymentOrderByOrderIdAsync(cancellationToken, orderId);

            var gw = await _gateway.CreateOrderAsync(new CreatePaymentOrderContext
            {
                OrderNo = orderFromDb!.OrderNo,
                Amount = amount,
                Currency = r.Currency,
                ReturnUrl = r.ReturnUrl,
                CallbackUrl = r.CallbackUrl
            }, cancellationToken);

            await _uow.FeeRepository.UpdatePaymentOrderStatusAsync(
                cancellationToken, orderFromDb.OrderId, "Pending", null, gw.GatewayOrderId, null);

            await _uow.FeeRepository.InsertPaymentGatewayEventAsync(cancellationToken, new PaymentGatewayEvent
            {
                OrderId = orderFromDb.OrderId,
                EventType = "Initiated",
                Payload = JsonSerializer.Serialize(new { r, gw })
            });

            //log OrderCreated with gateway order id
            await _uow.FeeRepository.InsertPaymentGatewayEventAsync(cancellationToken, new PaymentGatewayEvent
            {
                OrderId = orderFromDb.OrderId,
                EventType = "OrderCreated",
                Payload = JsonSerializer.Serialize(new { gateway = _gateway.Name, gatewayOrderId = gw.GatewayOrderId, amount, currency = r.Currency })
            });

            //log status transition -> Pending
            await _uow.FeeRepository.InsertPaymentGatewayEventAsync(cancellationToken, new PaymentGatewayEvent
            {
                OrderId = orderFromDb.OrderId,
                EventType = "StatusUpdate",
                Payload = JsonSerializer.Serialize(new { status = "Pending" })
            });

            return new InitiateOnlinePaymentResponseDto
            {
                OrderNo = orderFromDb.OrderNo,
                GatewayName = _gateway.Name,
                PaymentUrl = gw.PaymentUrl,
                Amount = amount,
                Currency = r.Currency
            };
        }
    }
}