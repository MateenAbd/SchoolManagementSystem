using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpdateLessonPlanCommandValidator : AbstractValidator<UpdateLessonPlanCommand>
    {
        public UpdateLessonPlanCommandValidator()
        {
            RuleFor(x => x.Plan.PlanId).GreaterThan(0);
            RuleFor(x => x.Plan.AcademicYear).NotEmpty();
            RuleFor(x => x.Plan.ClassName).NotEmpty();
            RuleFor(x => x.Plan.TeacherUserId).GreaterThan(0);
            RuleFor(x => x.Plan.PlanDate).NotEmpty();
            RuleFor(x => x.Plan.Topic).NotEmpty();
        }
    }
}