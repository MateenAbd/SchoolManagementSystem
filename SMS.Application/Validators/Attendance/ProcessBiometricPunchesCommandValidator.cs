using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class ProcessBiometricPunchesCommandValidator : AbstractValidator<ProcessBiometricPunchesCommand>
    {
        public ProcessBiometricPunchesCommandValidator()
        {
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty();
            RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate);
        }
    }
}