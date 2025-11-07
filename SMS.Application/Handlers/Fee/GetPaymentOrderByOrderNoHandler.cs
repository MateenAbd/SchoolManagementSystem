using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class GetPaymentOrderByOrderNoHandler : IRequestHandler<GetPaymentOrderByOrderNoQuery, PaymentGatewayOrder?>
    {
        private readonly IUnitOfWork _uow;
        public GetPaymentOrderByOrderNoHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<PaymentGatewayOrder?> Handle(GetPaymentOrderByOrderNoQuery request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.GetPaymentOrderByOrderNoAsync(cancellationToken, request.OrderNo);
    }
}