using System;

namespace SMS.Core.Entities
{
    public class AdmissionApplication
    {
        public int ApplicationId { get; set; }
        public int? InquiryId { get; set; }      // optional link from inquiry
        public string ApplicationNo { get; set; } = string.Empty; // generated if empty
        public string ApplicantName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public string? PreviousSchool { get; set; }
        public string ClassAppliedFor { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string Status { get; set; } = "Submitted"; // Submitted, UnderReview, Shortlisted, Rejected, Confirmed
        public decimal? TotalMarks { get; set; }
        public decimal? EntranceScore { get; set; }
        public string? Category { get; set; } // General/SC/ST/OBC/Other
        public bool DocumentsVerified { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}