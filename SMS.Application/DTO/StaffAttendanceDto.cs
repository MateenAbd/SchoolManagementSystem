using System;

namespace SMS.Application.Dto
{
    public class StaffAttendanceDto
    {
        public int AttendanceId { get; set; }
        public int UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } = "Present";
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public string? Remarks { get; set; }
        public int? MarkedByUserId { get; set; }
        public DateTime MarkedAtUtc { get; set; }
        public string? Source { get; set; }

        // Optional convenience (will get if from SQL if selected)
        public string? UserName { get; set; }
    }
}