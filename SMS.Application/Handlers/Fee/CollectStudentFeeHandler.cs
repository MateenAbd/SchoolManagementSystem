using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class CollectStudentFeeHandler : IRequestHandler<CollectStudentFeeCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public CollectStudentFeeHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<int> Handle(CollectStudentFeeCommand request, CancellationToken cancellationToken)
        {
            var r = request.Request;
            var total = r.Items?.Sum(i => i.Amount) ?? 0m;//0 decimal

            var header = new FeeReceipt
            {
                StudentId = r.StudentId,
                AcademicYear = r.AcademicYear,
                TermId = r.TermId,
                PaymentMode = r.PaymentMode,
                ReferenceNo = r.ReferenceNo,
                TotalAmount = total,
                ReceiptDate = r.ReceiptDate,
                ReceivedByUserId = r.ReceivedByUserId
            };

            var receiptId = await _uow.FeeRepository.CreateFeeReceiptAsync(cancellationToken, header);

            foreach (var item in r.Items)
            {
                var ri = new FeeReceiptItem
                {
                    ReceiptId = receiptId,
                    HeadId = item.HeadId,
                    Amount = item.Amount
                };
                await _uow.FeeRepository.AddFeeReceiptItemAsync(cancellationToken, ri);

                var credit = new StudentFeeLedger
                {
                    StudentId = r.StudentId,
                    AcademicYear = r.AcademicYear,
                    TermId = r.TermId,
                    HeadId = item.HeadId,
                    EntryType = "Credit",
                    Amount = item.Amount,
                    Narration = $"Receipt #{receiptId}",
                    ReceiptId = receiptId,
                    EntryDate = r.ReceiptDate
                };
                await _uow.FeeRepository.PostLedgerCreditAsync(cancellationToken, credit);
            }

            return receiptId;
        }
    }
}