using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class RejectStudentLeaveCommand : IRequest<int>
    {
        public int LeaveId { get; set; }
        public int ApprovedByUserId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
    }
}