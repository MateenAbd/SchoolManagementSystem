using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Services
{
    public class DummyPaymentGateway : IPaymentGateway
    {
        public string Name => "Dummy";

        public Task<CreatePaymentOrderResult> CreateOrderAsync(CreatePaymentOrderContext ctx, CancellationToken token = default)
        {
            // Simulate: return payment URL pointing to your app route (or a placeholder)
            var paymentUrl = $"{(ctx.ReturnUrl ?? "/Fee/Index")}#pay-{ctx.OrderNo}";
            // "GatewayOrderId" simulated
            return Task.FromResult(new CreatePaymentOrderResult($"DUMMY_GO_{ctx.OrderNo}", paymentUrl));
        }

        public Task<bool> VerifyCallbackAsync(VerifyPaymentContext ctx, CancellationToken token = default)
        {
            // For dummy, accept all "Success" callbacks
            return Task.FromResult(true);
        }
    }
}