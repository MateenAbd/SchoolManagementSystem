using System;

namespace SMS.Application.Dto
{
    public class FeeTermDto
    {
        public int TermId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;// e.g., "2025-26"
        public string TermCode { get; set; } = string.Empty;// e.g., "T1"
        public string TermName { get; set; } = string.Empty;// e.g., "Term 1"
        public int SequenceNo { get; set; }// e.g., 1 for first term
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; }
    }
}