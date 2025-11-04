using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class UpsertFeeDiscountSchemeCommandValidator : AbstractValidator<UpsertFeeDiscountSchemeCommand>
    {
        public UpsertFeeDiscountSchemeCommandValidator()
        {
            RuleFor(x => x.Scheme.SchemeCode).NotEmpty();
            RuleFor(x => x.Scheme.SchemeName).NotEmpty();
            RuleFor(x => x.Scheme.Mode).Must(m => m == "Percent" || m == "Amount");
            RuleFor(x => x.Scheme.Value).GreaterThan(0);
            RuleFor(x => x.Scheme.DiscountHeadId).GreaterThan(0);
        }
    }
}