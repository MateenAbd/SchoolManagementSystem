using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class UpdateStudentAttendanceStatusHandler : IRequestHandler<UpdateStudentAttendanceStatusCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public UpdateStudentAttendanceStatusHandler(IUnitOfWork uow)
        {
            _uow = uow; 
        }

        public Task<int> Handle(UpdateStudentAttendanceStatusCommand request, CancellationToken cancellationToken) =>
            _uow.AttendanceRepository.UpdateStudentAttendanceStatusAsync(cancellationToken, request.AttendanceId, request.Status, request.Remarks);
    }
}