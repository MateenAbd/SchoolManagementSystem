using System;

namespace SMS.Core.Entities
{
    public class FeeTerm
    {
        public int TermId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string TermCode { get; set; } = string.Empty; // unique per AY
        public string TermName { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}