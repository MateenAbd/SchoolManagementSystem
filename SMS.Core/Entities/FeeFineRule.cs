using System;

namespace SMS.Core.Entities
{
    public class FeeFineRule
    {
        public int RuleId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int TermId { get; set; }
        public int GraceDays { get; set; } = 0;     // days after due date
        public string Mode { get; set; } = "PerDayFixed"; // PerDayFixed/PerDayPercent/FixedOnce/PercentOnce
        public decimal Rate { get; set; }
        public decimal? MaxAmount { get; set; }
        public int FineHeadId { get; set; }         // FeeHeads.HeadId to post to
        public bool IsActive { get; set; } = true;
    }
}