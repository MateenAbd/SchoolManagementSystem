namespace SMS.Application.Dto
{
    public class PaymentCallbackDto
    {
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = "Success"; // Success/Failed/Cancelled
        public string? PaymentId { get; set; }
        public string? GatewayOrderId { get; set; }
        public string? Signature { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string? RawPayload { get; set; } // optional JSON for logging
    }
}