using System;

namespace SMS.Core.Entities
{
    public class StudentFeeLedger
    {
        public int LedgerId { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public int? HeadId { get; set; }              // for Credit/Debit head-based entries
        public string EntryType { get; set; } = "Debit"; // Debit/Credit
        public decimal Amount { get; set; }
        public string? Narration { get; set; }
        public int? ReceiptId { get; set; }           // for credits (payments)
        public DateTime EntryDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}