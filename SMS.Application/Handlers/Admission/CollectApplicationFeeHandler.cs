using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Admission
{
    public class CollectApplicationFeeHandler : IRequestHandler<CollectApplicationFeeCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public CollectApplicationFeeHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(CollectApplicationFeeCommand request, CancellationToken cancellationToken)
        {
            var payment = new AdmissionFeePayment
            {
                ApplicationId = request.ApplicationId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMode = request.PaymentMode,
                ReferenceNo = request.ReferenceNo,
                Remarks = request.Remarks,
                CollectedByUserId = request.CollectedByUserId,
                PaymentDate = request.PaymentDate
            };
            return _uow.AdmissionRepository.CollectApplicationFeeAsync(cancellationToken, payment);
        }
    }
}