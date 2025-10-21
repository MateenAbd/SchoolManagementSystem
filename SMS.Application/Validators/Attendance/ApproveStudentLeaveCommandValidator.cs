using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class ApproveStudentLeaveCommandValidator : AbstractValidator<ApproveStudentLeaveCommand>
    {
        public ApproveStudentLeaveCommandValidator()
        {
            RuleFor(x => x.LeaveId).GreaterThan(0);
            RuleFor(x => x.ApprovedByUserId).GreaterThan(0);
        }
    }
}