using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class CancelStudentLeaveHandler : IRequestHandler<CancelStudentLeaveCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public CancelStudentLeaveHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(CancelStudentLeaveCommand request, CancellationToken cancellationToken) =>
            _uow.AttendanceRepository.CancelStudentLeaveAsync(cancellationToken, request.LeaveId);
    }
}