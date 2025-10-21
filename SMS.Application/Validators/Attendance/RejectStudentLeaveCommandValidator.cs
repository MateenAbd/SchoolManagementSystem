using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class RejectStudentLeaveCommandValidator : AbstractValidator<RejectStudentLeaveCommand>
    {
        public RejectStudentLeaveCommandValidator()
        {
            RuleFor(x => x.LeaveId).GreaterThan(0);
            RuleFor(x => x.ApprovedByUserId).GreaterThan(0);
            RuleFor(x => x.RejectionReason).NotEmpty().MinimumLength(3);
        }
    }
}