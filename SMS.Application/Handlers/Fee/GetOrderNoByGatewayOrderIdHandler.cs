using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetOrderNoByGatewayOrderIdHandler : IRequestHandler<GetOrderNoByGatewayOrderIdQuery, string?>
    {
        private readonly IUnitOfWork _uow;
        public GetOrderNoByGatewayOrderIdHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<string?> Handle(GetOrderNoByGatewayOrderIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _uow.FeeRepository.GetPaymentOrderByGatewayOrderIdAsync(cancellationToken, request.GatewayOrderId);
            return order?.OrderNo;
        }
    }
}