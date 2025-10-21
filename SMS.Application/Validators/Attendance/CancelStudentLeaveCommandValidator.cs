using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class CancelStudentLeaveCommandValidator : AbstractValidator<CancelStudentLeaveCommand>
    {
        public CancelStudentLeaveCommandValidator()
        {
            RuleFor(x => x.LeaveId).GreaterThan(0);
        }
    }
}