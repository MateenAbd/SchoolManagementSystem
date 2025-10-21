using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class UpdateStudentAttendanceStatusCommand : IRequest<int>
    {
        public int AttendanceId { get; set; }
        public string Status { get; set; } = "Present";
        public string? Remarks { get; set; }
    }
}