using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class CancelStudentLeaveCommand : IRequest<int>
    {
        public int LeaveId { get; set; }
    }
}