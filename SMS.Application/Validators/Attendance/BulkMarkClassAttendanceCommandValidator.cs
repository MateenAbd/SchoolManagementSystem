using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class BulkMarkClassAttendanceCommandValidator : AbstractValidator<BulkMarkClassAttendanceCommand>
    {
        public BulkMarkClassAttendanceCommandValidator()
        {
            RuleFor(x => x.AttendanceDate).NotEmpty();
            RuleFor(x => x.ClassName).NotEmpty();
            RuleFor(x => x.Items).NotEmpty();
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.StudentId).GreaterThan(0);
                item.RuleFor(i => i.Status).NotEmpty();
            });
            RuleFor(x => x.PeriodNo).GreaterThan(0).When(x => x.SubjectCode != null);
        }
    }
}