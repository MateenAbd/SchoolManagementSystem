using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class CreateFeeTermCommandValidator : AbstractValidator<CreateFeeTermCommand>
    {
        public CreateFeeTermCommandValidator()
        {
            RuleFor(x => x.Term.AcademicYear).NotEmpty();
            RuleFor(x => x.Term.TermCode).NotEmpty();
            RuleFor(x => x.Term.TermName).NotEmpty();
            RuleFor(x => x.Term.SequenceNo).GreaterThan(0);
        }
    }
}