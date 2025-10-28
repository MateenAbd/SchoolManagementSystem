using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class ImportBiometricPunchesHandler : IRequestHandler<ImportBiometricPunchesCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ImportBiometricPunchesHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(ImportBiometricPunchesCommand request, CancellationToken cancellationToken)
        {
            var punches = request.Punches.Select(p => new BiometricRawPunch
            {
                DeviceId = p.DeviceId,
                ExternalUserId = p.ExternalUserId,
                PunchTime = p.PunchTime,
                Direction = p.Direction,
                Source = "Biometric"
            });
            return _uow.AttendanceRepository.ImportRawPunchesAsync(cancellationToken, punches);
        }
    }
}