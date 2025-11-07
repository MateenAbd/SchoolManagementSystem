namespace SMS.Admin.Models
{
    public class RazorpayCallbackForm
    {
        public string razorpay_payment_id { get; set; } = string.Empty;
        public string razorpay_order_id { get; set; } = string.Empty;
        public string razorpay_signature { get; set; } = string.Empty;
    }
}