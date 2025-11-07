using MediatR;

namespace SMS.Application.Queries.Fee
{
    public class GetPaymentOrderStatusQuery : IRequest<string?> // returns Status or null
    {
        public string OrderNo { get; set; } = string.Empty;
    }
}