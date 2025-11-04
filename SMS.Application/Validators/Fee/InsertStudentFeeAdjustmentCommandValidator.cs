using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class InsertStudentFeeAdjustmentCommandValidator : AbstractValidator<InsertStudentFeeAdjustmentCommand>
    {
        public InsertStudentFeeAdjustmentCommandValidator()
        {
            RuleFor(x => x.Adjustment.StudentId).GreaterThan(0);
            RuleFor(x => x.Adjustment.AcademicYear).NotEmpty();
            RuleFor(x => x.Adjustment.Type).Must(t => new[] { "Fine", "Discount", "Scholarship", "WriteOff" }.Contains(t));
            RuleFor(x => x.Adjustment.Amount).GreaterThan(0);
        }
    }
}