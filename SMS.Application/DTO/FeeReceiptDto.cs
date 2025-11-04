using System;

namespace SMS.Application.Dto
{
    public class FeeReceiptDto
    {
        public int ReceiptId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public string PaymentMode { get; set; } = "Cash";
        public string? ReferenceNo { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int? ReceivedByUserId { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}