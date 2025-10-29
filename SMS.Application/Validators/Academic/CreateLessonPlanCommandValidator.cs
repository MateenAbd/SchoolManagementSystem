using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class CreateLessonPlanCommandValidator : AbstractValidator<CreateLessonPlanCommand>
    {
        public CreateLessonPlanCommandValidator()
        {
            RuleFor(x => x.Plan.AcademicYear).NotEmpty();
            RuleFor(x => x.Plan.ClassName).NotEmpty();
            RuleFor(x => x.Plan.TeacherUserId).GreaterThan(0);
            RuleFor(x => x.Plan.PlanDate).NotEmpty();
            RuleFor(x => x.Plan.Topic).NotEmpty();
            RuleFor(x => x.Plan).Must(p =>
                (p.CourseId.HasValue && p.CourseId > 0) || (p.SubjectId.HasValue && p.SubjectId > 0))
                .WithMessage("Either CourseId or SubjectId must be provided.");
            RuleFor(x => x.Plan).Must(p =>
                (p.PeriodNo.HasValue && !p.StartTime.HasValue && !p.EndTime.HasValue) ||
                (!p.PeriodNo.HasValue && p.StartTime.HasValue && p.EndTime.HasValue) ||
                (!p.PeriodNo.HasValue && !p.StartTime.HasValue && !p.EndTime.HasValue))
                .WithMessage("Provide PeriodNo or Start/End time, or none.");
            RuleFor(x => x.Plan.EndTime).GreaterThan(x => x.Plan.StartTime)
                .When(x => x.Plan.StartTime.HasValue && x.Plan.EndTime.HasValue);
        }
    }
}