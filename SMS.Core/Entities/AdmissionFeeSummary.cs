namespace SMS.Core.Entities
{
    public class AdmissionFeeSummary
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassAppliedFor { get; set; } = string.Empty;
        public int ApplicationsCount { get; set; }
        public int PaymentsCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}