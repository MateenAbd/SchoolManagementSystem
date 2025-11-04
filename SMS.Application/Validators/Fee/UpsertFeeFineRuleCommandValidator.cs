using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class UpsertFeeFineRuleCommandValidator : AbstractValidator<UpsertFeeFineRuleCommand>
    {
        public UpsertFeeFineRuleCommandValidator()
        {
            RuleFor(x => x.Rule.AcademicYear).NotEmpty();
            RuleFor(x => x.Rule.TermId).GreaterThan(0);
            RuleFor(x => x.Rule.GraceDays).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Rule.Mode).Must(m => new[] { "PerDayFixed", "PerDayPercent", "FixedOnce", "PercentOnce" }.Contains(m));
            RuleFor(x => x.Rule.Rate).GreaterThan(0);
            RuleFor(x => x.Rule.FineHeadId).GreaterThan(0);
        }
    }
}