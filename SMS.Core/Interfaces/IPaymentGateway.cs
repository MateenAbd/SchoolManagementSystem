using System.Threading;
using System.Threading.Tasks;

namespace SMS.Core.Interfaces
{
    public record CreatePaymentOrderContext
    {
        public string OrderNo { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "INR";
        public string? ReturnUrl { get; init; }
        public string? CallbackUrl { get; init; }
    }

    public record CreatePaymentOrderResult(string GatewayOrderId, string PaymentUrl);

    public record VerifyPaymentContext
    {
        public string OrderNo { get; init; } = string.Empty;
        public string? PaymentId { get; init; }
        public string? GatewayOrderId { get; init; }
        public string? Signature { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "INR";
        public string? RawPayload { get; init; }
    }

    public interface IPaymentGateway
    {
        string Name { get; }
        Task<CreatePaymentOrderResult> CreateOrderAsync(CreatePaymentOrderContext ctx, CancellationToken token = default);
        Task<bool> VerifyCallbackAsync(VerifyPaymentContext ctx, CancellationToken token = default);
    }
}