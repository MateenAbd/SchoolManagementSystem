using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class UpsertFeeStructureCommandValidator : AbstractValidator<UpsertFeeStructureCommand>
    {
        public UpsertFeeStructureCommandValidator()
        {
            RuleFor(x => x.Structure.AcademicYear).NotEmpty();
            RuleFor(x => x.Structure.ClassName).NotEmpty();
            RuleFor(x => x.Structure.TermId).GreaterThan(0);
            RuleForEach(x => x.Structure.Details).ChildRules(d =>
            {
                d.RuleFor(i => i.HeadId).GreaterThan(0);
                d.RuleFor(i => i.Amount).GreaterThanOrEqualTo(0);
            });
        }
    }
}