using System;

namespace SMS.Application.Dto
{
    public class AdmissionInquiryDto
    {
        public int InquiryId { get; set; }
        public string Source { get; set; } = "Online";
        public string LeadStatus { get; set; } = "New";
        public string ApplicantName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string InterestedClass { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}