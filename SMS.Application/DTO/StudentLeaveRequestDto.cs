using System;

namespace SMS.Application.Dto
{
    public class StudentLeaveRequestDto
    {
        public int LeaveId { get; set; }
        public int StudentId { get; set; }
        public string LeaveType { get; set; } = "Sick";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending";
        public int? AppliedByUserId { get; set; }
        public DateTime AppliedAtUtc { get; set; }
        public int? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? RejectionReason { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}