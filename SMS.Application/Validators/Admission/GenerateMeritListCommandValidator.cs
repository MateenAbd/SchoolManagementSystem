using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class GenerateMeritListCommandValidator : AbstractValidator<GenerateMeritListCommand>
    {
        public GenerateMeritListCommandValidator()
        {
            RuleFor(x => x.AcademicYear).NotEmpty();
            RuleFor(x => x.ClassAppliedFor).NotEmpty();
            RuleFor(x => x.TopN).GreaterThan(0).When(x => x.TopN.HasValue);
        }
    }
}