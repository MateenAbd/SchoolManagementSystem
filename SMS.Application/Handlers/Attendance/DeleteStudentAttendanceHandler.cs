using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class DeleteStudentAttendanceHandler : IRequestHandler<DeleteStudentAttendanceCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteStudentAttendanceHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(DeleteStudentAttendanceCommand request, CancellationToken cancellationToken)
        {
            return _uow.AttendanceRepository.DeleteStudentAttendanceAsync(cancellationToken, request.AttendanceId);
        }
    }
}