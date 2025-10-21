using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class ApplyStudentLeaveHandler : IRequestHandler<ApplyStudentLeaveCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ApplyStudentLeaveHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(ApplyStudentLeaveCommand request, CancellationToken cancellationToken)
        {
            var entity = new StudentLeaveRequest
            {
                StudentId = request.StudentId,
                LeaveType = request.LeaveType,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                Reason = request.Reason,
                AppliedByUserId = request.AppliedByUserId
            };
            return _uow.AttendanceRepository.ApplyStudentLeaveAsync(cancellationToken, entity);
        }
    }
}