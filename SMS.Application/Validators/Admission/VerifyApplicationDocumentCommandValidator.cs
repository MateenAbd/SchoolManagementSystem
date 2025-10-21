using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class VerifyApplicationDocumentCommandValidator : AbstractValidator<VerifyApplicationDocumentCommand>
    {
        public VerifyApplicationDocumentCommandValidator()
        {
            RuleFor(x => x.DocumentId).GreaterThan(0);
            RuleFor(x => x.VerifiedByUserId).GreaterThan(0);
        }
    }
}