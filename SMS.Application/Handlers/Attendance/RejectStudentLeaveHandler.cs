using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class RejectStudentLeaveHandler : IRequestHandler<RejectStudentLeaveCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public RejectStudentLeaveHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(RejectStudentLeaveCommand request, CancellationToken cancellationToken) =>
            _uow.AttendanceRepository.RejectStudentLeaveAsync(cancellationToken, request.LeaveId, request.ApprovedByUserId, request.RejectionReason);
    }
}