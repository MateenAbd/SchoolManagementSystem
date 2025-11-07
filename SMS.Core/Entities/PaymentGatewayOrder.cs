using System;

namespace SMS.Core.Entities
{
    public class PaymentGatewayOrder
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;// e.g.,PG-20250410-000123
        public string GatewayName { get; set; } = "Dummy"; //Razorpay/Stripe/Dummy...
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Status { get; set; } = "Initiated";  //Initiated/Pending/Success/Failed/Cancelled
        public string? GatewayOrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? PaymentMode { get; set; } // OnlineGateway
        public string? ReferenceNo { get; set; }// gateway payment ref
        public string? ReturnUrl { get; set; }
        public string? CallbackUrl { get; set; }
        public string ItemsJson { get; set; } = "[]";// FeeReceiptItem list as JSON
        public int? ReceiptId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}