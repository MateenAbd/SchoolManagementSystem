using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class UpdateStaffAttendanceStatusCommandValidator : AbstractValidator<UpdateStaffAttendanceStatusCommand>
    {
        public UpdateStaffAttendanceStatusCommandValidator()
        {
            RuleFor(x => x.AttendanceId).GreaterThan(0);
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.OutTime).GreaterThan(x => x.InTime).When(x => x.InTime.HasValue && x.OutTime.HasValue);
        }
    }
}