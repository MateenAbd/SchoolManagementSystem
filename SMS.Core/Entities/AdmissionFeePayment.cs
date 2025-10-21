using System;

namespace SMS.Core.Entities
{
    public class AdmissionFeePayment
    {
        public int PaymentId { get; set; }
        public int ApplicationId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMode { get; set; } = "Cash"; // Cash, Card, UPI, NetBanking, Cheque
        public string? ReferenceNo { get; set; } // txn id / cheque no
        public string? Remarks { get; set; }
        public int? CollectedByUserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}