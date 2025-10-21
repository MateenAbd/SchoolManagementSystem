using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class ConfirmAdmissionCommandValidator : AbstractValidator<ConfirmAdmissionCommand>
    {
        public ConfirmAdmissionCommandValidator()
        {
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.EnrollmentDate).NotEmpty();
        }
    }
}