using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class AddApplicationDocumentCommandValidator : AbstractValidator<AddApplicationDocumentCommand>
    {
        public AddApplicationDocumentCommandValidator()
        {
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.FileName).NotEmpty();
            RuleFor(x => x.FilePath).NotEmpty();
        }
    }
}