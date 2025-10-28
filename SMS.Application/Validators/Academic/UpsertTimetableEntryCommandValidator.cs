using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpsertTimetableEntryCommandValidator : AbstractValidator<UpsertTimetableEntryCommand>
    {
        public UpsertTimetableEntryCommandValidator()
        {
            RuleFor(x => x.Entry.AcademicYear).NotEmpty();
            RuleFor(x => x.Entry.ClassName).NotEmpty();
            RuleFor(x => x.Entry.DayOfWeek).InclusiveBetween((byte)1, (byte)7);
            RuleFor(x => x.Entry.SubjectId).GreaterThan(0);
            RuleFor(x => x.Entry).Must(e =>
                (e.PeriodNo.HasValue && !e.StartTime.HasValue && !e.EndTime.HasValue) ||
                (!e.PeriodNo.HasValue && e.StartTime.HasValue && e.EndTime.HasValue))
                .WithMessage("Provide either PeriodNo or StartTime+EndTime.");
            RuleFor(x => x.Entry.EndTime).GreaterThan(x => x.Entry.StartTime)
                .When(x => x.Entry.StartTime.HasValue && x.Entry.EndTime.HasValue);
        }
    }
}