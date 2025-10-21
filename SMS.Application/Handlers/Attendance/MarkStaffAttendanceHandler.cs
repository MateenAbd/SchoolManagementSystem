using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class MarkStaffAttendanceHandler : IRequestHandler<MarkStaffAttendanceCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public MarkStaffAttendanceHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(MarkStaffAttendanceCommand request, CancellationToken cancellationToken)
        {
            var entity = new StaffAttendance
            {
                UserId = request.UserId,
                AttendanceDate = request.AttendanceDate.Date,
                Status = request.Status,
                InTime = request.InTime,
                OutTime = request.OutTime,
                Remarks = request.Remarks,
                MarkedByUserId = request.MarkedByUserId,
                Source = request.Source
            };
            return _uow.AttendanceRepository.UpsertStaffAttendanceAsync(cancellationToken, entity);
        }
    }
}