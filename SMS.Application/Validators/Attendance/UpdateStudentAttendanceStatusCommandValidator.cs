using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class UpdateStudentAttendanceStatusCommandValidator : AbstractValidator<UpdateStudentAttendanceStatusCommand>
    {
        public UpdateStudentAttendanceStatusCommandValidator()
        {
            RuleFor(x => x.AttendanceId).GreaterThan(0);
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}