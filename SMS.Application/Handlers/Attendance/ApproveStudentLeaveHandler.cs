using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class ApproveStudentLeaveHandler : IRequestHandler<ApproveStudentLeaveCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ApproveStudentLeaveHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(ApproveStudentLeaveCommand request, CancellationToken cancellationToken)
        {
            return _uow.AttendanceRepository.ApproveStudentLeaveAsync(cancellationToken, request.LeaveId, request.ApprovedByUserId);
        }
    }
}