using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class UpdateStaffAttendanceStatusHandler : IRequestHandler<UpdateStaffAttendanceStatusCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public UpdateStaffAttendanceStatusHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(UpdateStaffAttendanceStatusCommand request, CancellationToken cancellationToken) =>
            _uow.AttendanceRepository.UpdateStaffAttendanceStatusAsync(cancellationToken, request.AttendanceId, request.Status, request.Remarks, request.InTime, request.OutTime);
    }
}