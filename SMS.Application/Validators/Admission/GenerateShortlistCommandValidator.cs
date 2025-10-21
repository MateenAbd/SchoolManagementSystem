using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class GenerateShortlistCommandValidator : AbstractValidator<GenerateShortlistCommand>
    {
        public GenerateShortlistCommandValidator()
        {
            RuleFor(x => x.AcademicYear).NotEmpty();
            RuleFor(x => x.ClassAppliedFor).NotEmpty();
            RuleFor(x => x.TopN).GreaterThan(0).When(x => x.TopN.HasValue);
            RuleFor(x => x.MinEntranceScore).GreaterThanOrEqualTo(0).When(x => x.MinEntranceScore.HasValue);
        }
    }
}