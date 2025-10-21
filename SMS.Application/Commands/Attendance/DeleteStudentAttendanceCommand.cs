using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class DeleteStudentAttendanceCommand : IRequest<int>
    {
        public int AttendanceId { get; set; }
    }
}