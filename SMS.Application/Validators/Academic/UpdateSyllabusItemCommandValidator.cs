using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpdateSyllabusItemCommandValidator : AbstractValidator<UpdateSyllabusItemCommand>
    {
        public UpdateSyllabusItemCommandValidator()
        {
            RuleFor(x => x.Item.SyllabusId).GreaterThan(0);
            RuleFor(x => x.Item.CourseId).GreaterThan(0);
            RuleFor(x => x.Item.Topic).NotEmpty();
        }
    }
}