using System;

namespace SMS.Application.Dto
{
    public class StudentFeeLedgerDto
    {
        public int LedgerId { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public int? HeadId { get; set; }
        public string EntryType { get; set; } = "Debit";
        public decimal Amount { get; set; }
        public string? Narration { get; set; }
        public int? ReceiptId { get; set; }
        public decimal? Balance { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}