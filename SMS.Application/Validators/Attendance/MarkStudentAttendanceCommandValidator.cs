using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class MarkStudentAttendanceCommandValidator : AbstractValidator<MarkStudentAttendanceCommand>
    {
        public MarkStudentAttendanceCommandValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.AttendanceDate).NotEmpty();
            RuleFor(x => x.ClassName).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.PeriodNo).GreaterThan(0).When(x => x.SubjectCode != null);
        }
    }
}