namespace SMS.Application.Dto
{
    public class InitiateOnlinePaymentResponseDto
    {
        public string OrderNo { get; set; } = string.Empty;
        public string GatewayName { get; set; } = "Dummy";
        public string PaymentUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}