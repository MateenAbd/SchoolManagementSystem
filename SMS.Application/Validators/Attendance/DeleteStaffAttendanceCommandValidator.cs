using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class DeleteStaffAttendanceCommandValidator : AbstractValidator<DeleteStaffAttendanceCommand>
    {
        public DeleteStaffAttendanceCommandValidator()
        {
            RuleFor(x => x.AttendanceId).GreaterThan(0);
        }
    }
}