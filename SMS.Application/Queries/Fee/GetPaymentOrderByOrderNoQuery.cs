using MediatR;
using SMS.Core.Entities;

namespace SMS.Application.Queries.Fee
{
    public class GetPaymentOrderByOrderNoQuery : IRequest<PaymentGatewayOrder?>
    {
        public string OrderNo { get; set; } = string.Empty;
    }
}