using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class ApplyStudentLeaveCommandValidator : AbstractValidator<ApplyStudentLeaveCommand>
    {
        public ApplyStudentLeaveCommandValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.LeaveType).NotEmpty();
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty();
            RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate);
        }
    }
}