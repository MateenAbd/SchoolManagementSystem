using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class AutoMarkApprovedLeavesHandler : IRequestHandler<AutoMarkApprovedLeavesCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public AutoMarkApprovedLeavesHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> Handle(AutoMarkApprovedLeavesCommand request, CancellationToken cancellationToken)
        {
            int affected = 0;
            if (request.IncludeStudents)
                affected += await _uow.AttendanceRepository.AutoMarkStudentApprovedLeavesRangeAsync(cancellationToken, request.FromDate.Date, request.ToDate.Date);
            if (request.IncludeStaff)
                affected += await _uow.AttendanceRepository.AutoMarkStaffApprovedLeavesRangeAsync(cancellationToken, request.FromDate.Date, request.ToDate.Date);
            return affected;
        }
    }
}