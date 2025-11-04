using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class CreateFeeHeadCommandValidator : AbstractValidator<CreateFeeHeadCommand>
    {
        public CreateFeeHeadCommandValidator()
        {
            RuleFor(x => x.Head.HeadCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Head.HeadName).NotEmpty().MaximumLength(200);
        }
    }
}