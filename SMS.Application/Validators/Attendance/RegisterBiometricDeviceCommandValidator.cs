using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class RegisterBiometricDeviceCommandValidator : AbstractValidator<RegisterBiometricDeviceCommand>
    {
        public RegisterBiometricDeviceCommandValidator()
        {
            RuleFor(x => x.Device.Name).NotEmpty();
            RuleFor(x => x.Device.SerialNo).NotEmpty();
        }
    }
}