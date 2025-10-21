using System;
using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class UpdateStaffAttendanceStatusCommand : IRequest<int>
    {
        public int AttendanceId { get; set; }
        public string Status { get; set; } = "Present";
        public string? Remarks { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
    }
}