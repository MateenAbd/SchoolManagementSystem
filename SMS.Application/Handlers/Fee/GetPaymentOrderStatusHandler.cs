using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Queries.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class GetPaymentOrderStatusHandler : IRequestHandler<GetPaymentOrderStatusQuery, string?>
    {
        private readonly IUnitOfWork _uow;
        public GetPaymentOrderStatusHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<string?> Handle(GetPaymentOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var order = await _uow.FeeRepository.GetPaymentOrderByOrderNoAsync(cancellationToken, request.OrderNo);
            return order?.Status;
        }
    }
}