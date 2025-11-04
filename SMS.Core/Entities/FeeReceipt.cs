using System;

namespace SMS.Core.Entities
{
    public class FeeReceipt
    {
        public int ReceiptId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public string PaymentMode { get; set; } = "Cash"; // Cash/Card/UPI/NetBanking/OnlineGateway/Cheque/DD
        public string? ReferenceNo { get; set; }          // txn id / cheque no, etc.
        public decimal TotalAmount { get; set; }          // sum of items
        public DateTime ReceiptDate { get; set; }
        public int? ReceivedByUserId { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}