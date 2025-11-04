namespace SMS.Core.Entities
{
    public class StudentScholarship
    {
        public int ScholarshipId { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int? TermId { get; set; }
        public int? SchemeId { get; set; }           // optional link to scheme
        public string Mode { get; set; } = "Percent"; // Percent/Amount
        public decimal Value { get; set; }
        public decimal? CapAmount { get; set; }
        public int ScholarshipHeadId { get; set; }   // FeeHeads.HeadId to post to
        public bool IsActive { get; set; } = true;
    }
}