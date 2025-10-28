using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class ImportBiometricPunchesCommandValidator : AbstractValidator<ImportBiometricPunchesCommand>
    {
        public ImportBiometricPunchesCommandValidator()
        {
            RuleFor(x => x.Punches).NotEmpty();
            RuleForEach(x => x.Punches).ChildRules(p =>
            {
                p.RuleFor(i => i.DeviceId).GreaterThan(0);
                p.RuleFor(i => i.ExternalUserId).NotEmpty();
                p.RuleFor(i => i.PunchTime).NotEmpty();
            });
        }
    }
}