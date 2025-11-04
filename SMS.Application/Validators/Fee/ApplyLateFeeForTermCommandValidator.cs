using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class ApplyLateFeeForTermCommandValidator : AbstractValidator<ApplyLateFeeForTermCommand>
    {
        public ApplyLateFeeForTermCommandValidator()
        {
            RuleFor(x => x.AcademicYear).NotEmpty();
            RuleFor(x => x.TermId).GreaterThan(0);
        }
    }
}