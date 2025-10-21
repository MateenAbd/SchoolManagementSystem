using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class DeleteStaffAttendanceCommand : IRequest<int>
    {
        public int AttendanceId { get; set; }
    }
}