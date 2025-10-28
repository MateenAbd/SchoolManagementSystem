using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
    {
        public UpdateSubjectCommandValidator()
        {
            RuleFor(x => x.Subject.SubjectId).GreaterThan(0);
            RuleFor(x => x.Subject.SubjectCode).NotEmpty();
            RuleFor(x => x.Subject.SubjectName).NotEmpty();
        }
    }
}