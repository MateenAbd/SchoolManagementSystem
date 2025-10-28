using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class AddSyllabusItemCommandValidator : AbstractValidator<AddSyllabusItemCommand>
    {
        public AddSyllabusItemCommandValidator()
        {
            RuleFor(x => x.Item.CourseId).GreaterThan(0);
            RuleFor(x => x.Item.Topic).NotEmpty();
        }
    }
}