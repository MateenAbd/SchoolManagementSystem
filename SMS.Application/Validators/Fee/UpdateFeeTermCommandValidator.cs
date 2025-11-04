using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class UpdateFeeTermCommandValidator : AbstractValidator<UpdateFeeTermCommand>
    {
        public UpdateFeeTermCommandValidator()
        {
            RuleFor(x => x.Term.TermId).GreaterThan(0);
            RuleFor(x => x.Term.TermCode).NotEmpty();
            RuleFor(x => x.Term.TermName).NotEmpty();
            RuleFor(x => x.Term.SequenceNo).GreaterThan(0);
        }
    }
}