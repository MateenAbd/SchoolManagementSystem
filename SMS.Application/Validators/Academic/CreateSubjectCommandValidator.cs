using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
    {
        public CreateSubjectCommandValidator()
        {
            RuleFor(x => x.Subject.SubjectCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Subject.SubjectName).NotEmpty().MaximumLength(200);
        }
    }
}