using System;

namespace SMS.Application.Dto
{
    public class AdmissionFeePaymentDto
    {
        public int PaymentId { get; set; }
        public int ApplicationId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMode { get; set; } = "Cash";
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }
        public int? CollectedByUserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}