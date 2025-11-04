using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class UpdateFeeHeadCommandValidator : AbstractValidator<UpdateFeeHeadCommand>
    {
        public UpdateFeeHeadCommandValidator()
        {
            RuleFor(x => x.Head.HeadId).GreaterThan(0);
            RuleFor(x => x.Head.HeadCode).NotEmpty();
            RuleFor(x => x.Head.HeadName).NotEmpty();
        }
    }
}