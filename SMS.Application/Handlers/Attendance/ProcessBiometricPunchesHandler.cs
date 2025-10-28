using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class ProcessBiometricPunchesHandler : IRequestHandler<ProcessBiometricPunchesCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ProcessBiometricPunchesHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(ProcessBiometricPunchesCommand request, CancellationToken cancellationToken) =>
            _uow.AttendanceRepository.ProcessBiometricPunchesAsync(cancellationToken, request.FromDate.Date, request.ToDate.Date);
    }
}