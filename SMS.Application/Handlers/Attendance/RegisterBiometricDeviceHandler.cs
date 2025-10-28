using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class RegisterBiometricDeviceHandler : IRequestHandler<RegisterBiometricDeviceCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public RegisterBiometricDeviceHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(RegisterBiometricDeviceCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<BiometricDevice>(request.Device);
            return _uow.AttendanceRepository.RegisterBiometricDeviceAsync(cancellationToken, entity);
        }
    }
}