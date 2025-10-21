using System;

namespace SMS.Core.Entities
{
    public class AdmissionInquiry
    {
        public int InquiryId { get; set; }
        public string Source { get; set; } = "Online"; // Online, Walk-in, Phone, Referral
        public string LeadStatus { get; set; } = "New"; // New, Contacted, Applied, Dropped
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