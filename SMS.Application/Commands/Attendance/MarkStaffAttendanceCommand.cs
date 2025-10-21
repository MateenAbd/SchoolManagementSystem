using System;
using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class MarkStaffAttendanceCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } = "Present"; // Present/Absent/Late/Excused
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public string? Remarks { get; set; }
        public int? MarkedByUserId { get; set; }
        public string? Source { get; set; } = "Manual"; // Manual/Biometric/RFID
    }
}