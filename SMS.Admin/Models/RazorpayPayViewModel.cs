namespace SMS.Admin.Models
{
    public class RazorpayPayViewModel
    {
        public string OrderNo { get; set; } = string.Empty;
        public string GatewayOrderId { get; set; } = string.Empty;
        public string KeyId { get; set; } = string.Empty;
        public string Currency { get; set; } = "INR";
        public int AmountPaise { get; set; }
        public string CallbackUrl { get; set; } = string.Empty;
        public string DisplayTitle { get; set; } = "Fees Payment";
    }
}