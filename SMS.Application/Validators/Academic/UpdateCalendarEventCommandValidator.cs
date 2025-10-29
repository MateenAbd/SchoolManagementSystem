using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpdateCalendarEventCommandValidator : AbstractValidator<UpdateCalendarEventCommand>
    {
        public UpdateCalendarEventCommandValidator()
        {
            RuleFor(x => x.Event.EventId).GreaterThan(0);
            RuleFor(x => x.Event.Title).NotEmpty();
            RuleFor(x => x.Event.EventType).NotEmpty();
            RuleFor(x => x.Event.StartDate).NotEmpty();
            RuleFor(x => x.Event.EndDate).GreaterThanOrEqualTo(x => x.Event.StartDate)
                .When(x => x.Event.EndDate.HasValue);
        }
    }
}