using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class DeleteStaffAttendanceHandler : IRequestHandler<DeleteStaffAttendanceCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteStaffAttendanceHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(DeleteStaffAttendanceCommand request, CancellationToken cancellationToken) =>
            _uow.AttendanceRepository.DeleteStaffAttendanceAsync(cancellationToken, request.AttendanceId);
    }
}