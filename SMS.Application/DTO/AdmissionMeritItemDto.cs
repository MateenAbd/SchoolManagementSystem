using System;

namespace SMS.Application.Dto
{
    public class AdmissionMeritItemDto
    {
        public int MeritId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationNo { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassAppliedFor { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int Rank { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string? Notes { get; set; }
    }
}