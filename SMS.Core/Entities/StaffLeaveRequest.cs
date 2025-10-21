using System;

namespace SMS.Core.Entities
{
    public class StaffLeaveRequest
    {
        public int LeaveId { get; set; }
        public int UserId { get; set; }
        public string LeaveType { get; set; } = "Sick"; // Sick/Casual/Other
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected/Cancelled
        public int? AppliedByUserId { get; set; }
        public DateTime AppliedAtUtc { get; set; }
        public int? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? RejectionReason { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}