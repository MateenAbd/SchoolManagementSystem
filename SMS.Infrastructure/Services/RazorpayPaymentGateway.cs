using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Services
{
    public class RazorpayPaymentGateway : IPaymentGateway
    {
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly RazorpayClient _client;

        public string Name => "Razorpay";

        public RazorpayPaymentGateway(IConfiguration config)
        {
            _keyId = config["Payments:Razorpay:KeyId"] ?? "";
            _keySecret = config["Payments:Razorpay:KeySecret"] ?? "";
            _client = new RazorpayClient(_keyId, _keySecret);
        }

        public Task<CreatePaymentOrderResult> CreateOrderAsync(CreatePaymentOrderContext ctx, CancellationToken token = default)
        {
            var amountPaise = (int)System.Math.Round(ctx.Amount * 100m, 0);
            var options = new System.Collections.Generic.Dictionary<string, object>
            {
                { "amount", amountPaise },
                { "currency", ctx.Currency },
                { "receipt", ctx.OrderNo },
                { "payment_capture", 1 }
            };
            var order = _client.Order.Create(options);
            var gatewayOrderId = order["id"].ToString();

            // Payment URL will be handled by our Pay page; return empty here
            return Task.FromResult(new CreatePaymentOrderResult(gatewayOrderId, PaymentUrl: ""));
        }

        public Task<bool> VerifyCallbackAsync(VerifyPaymentContext ctx, CancellationToken token = default)
        {
            // Razorpay signature = HMAC_SHA256(order_id|payment_id, key_secret), hex lowercase
            if (string.IsNullOrWhiteSpace(ctx.GatewayOrderId) || string.IsNullOrWhiteSpace(ctx.PaymentId) || string.IsNullOrWhiteSpace(ctx.Signature))
                return Task.FromResult(false);

            var payload = $"{ctx.GatewayOrderId}|{ctx.PaymentId}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generated = System.BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            return Task.FromResult(generated == ctx.Signature.ToLowerInvariant());
        }
    }
}