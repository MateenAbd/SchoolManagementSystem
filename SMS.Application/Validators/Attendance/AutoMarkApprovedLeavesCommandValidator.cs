using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class AutoMarkApprovedLeavesCommandValidator : AbstractValidator<AutoMarkApprovedLeavesCommand>
    {
        public AutoMarkApprovedLeavesCommandValidator()
        {
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty();
            RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate);
            RuleFor(x => x.IncludeStudents).Equal(true).Unless(x => x.IncludeStaff)
                .WithMessage("At least one of IncludeStudents or IncludeStaff must be true.");
        }
    }
}