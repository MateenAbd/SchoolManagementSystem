using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class CreateCalendarEventCommandValidator : AbstractValidator<CreateCalendarEventCommand>
    {
        public CreateCalendarEventCommandValidator()
        {
            RuleFor(x => x.Event.AcademicYear).NotEmpty();
            RuleFor(x => x.Event.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Event.EventType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Event.StartDate).NotEmpty();
            RuleFor(x => x.Event.EndDate).GreaterThanOrEqualTo(x => x.Event.StartDate)
                .When(x => x.Event.EndDate.HasValue);
        }
    }
}