using MediatR;

namespace SMS.Application.Queries.Fee
{
    public class GetOrderNoByGatewayOrderIdQuery : IRequest<string?>
    {
        public string GatewayOrderId { get; set; } = string.Empty;
    }
}