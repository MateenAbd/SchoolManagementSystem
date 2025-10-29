using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpdateLessonPlanStatusCommandValidator : AbstractValidator<UpdateLessonPlanStatusCommand>
    {
        public UpdateLessonPlanStatusCommandValidator()
        {
            RuleFor(x => x.PlanId).GreaterThan(0);
            RuleFor(x => x.Status).NotEmpty().Must(s => s == "Draft" || s == "Planned" || s == "Delivered");
        }
    }
}