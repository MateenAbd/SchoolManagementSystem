using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class CollectApplicationFeeCommandValidator : AbstractValidator<CollectApplicationFeeCommand>
    {
        public CollectApplicationFeeCommandValidator()
        {
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
            RuleFor(x => x.PaymentMode).NotEmpty();
        }
    }
}