using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class ApplyDiscountForStudentTermCommandValidator : AbstractValidator<ApplyDiscountForStudentTermCommand>
    {
        public ApplyDiscountForStudentTermCommandValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.AcademicYear).NotEmpty();
            RuleFor(x => x.TermId).GreaterThan(0);
        }
    }
}