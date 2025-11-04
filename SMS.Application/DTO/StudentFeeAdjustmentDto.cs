using System;

namespace SMS.Application.Dto
{
    public class StudentFeeAdjustmentDto
    {
        public int AdjustmentId { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int? TermId { get; set; }
        public int? HeadId { get; set; }// optional; required for fines/discounts if you want head-based reporting
        public string Type { get; set; } = "Discount"; // Fine/Discount/Scholarship/WriteOff
        public decimal Amount { get; set; }
        public string? Narration { get; set; }
        public DateTime EntryDate { get; set; }
        public int? CreatedByUserId { get; set; }
    }
}