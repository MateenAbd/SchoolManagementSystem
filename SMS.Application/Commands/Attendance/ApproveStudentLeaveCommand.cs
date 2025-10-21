using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class ApproveStudentLeaveCommand : IRequest<int>
    {
        public int LeaveId { get; set; }
        public int ApprovedByUserId { get; set; }
    }
}