namespace SMS.Core.Entities
{
    public class FeeDiscountScheme
    {
        public int SchemeId { get; set; }
        public string SchemeCode { get; set; } = string.Empty;
        public string SchemeName { get; set; } = string.Empty;
        public string? AcademicYear { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int? TermId { get; set; }
        public string Mode { get; set; } = "Percent"; // Percent/Amount
        public decimal Value { get; set; }            // percent or amount
        public decimal? CapAmount { get; set; }
        public int DiscountHeadId { get; set; }       // FeeHeads.HeadId to post to
        public bool IsActive { get; set; } = true;
    }
}