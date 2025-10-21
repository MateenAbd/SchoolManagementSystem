using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
    {
        public CreateApplicationCommandValidator()
        {
            RuleFor(x => x.Application.ApplicantName).NotEmpty();
            RuleFor(x => x.Application.ClassAppliedFor).NotEmpty();
            RuleFor(x => x.Application.AcademicYear).NotEmpty();
        }
    }
}