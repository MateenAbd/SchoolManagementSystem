using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class UpsertBiometricUserMapCommandValidator : AbstractValidator<UpsertBiometricUserMapCommand>
    {
        public UpsertBiometricUserMapCommandValidator()
        {
            RuleFor(x => x.Map.DeviceId).GreaterThan(0);
            RuleFor(x => x.Map.ExternalUserId).NotEmpty();
            RuleFor(x => x.Map.PersonType).Must(p => p == "Student" || p == "Staff");
            When(x => x.Map.PersonType == "Student", () => RuleFor(x => x.Map.StudentId).NotNull().GreaterThan(0));
            When(x => x.Map.PersonType == "Staff", () => RuleFor(x => x.Map.UserId).NotNull().GreaterThan(0));
        }
    }
}