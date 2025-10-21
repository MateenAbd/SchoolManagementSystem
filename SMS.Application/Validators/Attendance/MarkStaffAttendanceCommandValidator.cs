using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class MarkStaffAttendanceCommandValidator : AbstractValidator<MarkStaffAttendanceCommand>
    {
        public MarkStaffAttendanceCommandValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.AttendanceDate).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.OutTime).GreaterThan(x => x.InTime).When(x => x.InTime.HasValue && x.OutTime.HasValue);
        }
    }
}