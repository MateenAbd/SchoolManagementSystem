using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Attendance
{
    public class RegisterBiometricDeviceCommand : IRequest<int>
    {
        public BiometricDeviceDto Device { get; set; } = new();
    }
}