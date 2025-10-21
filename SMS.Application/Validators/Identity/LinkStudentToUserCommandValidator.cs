using FluentValidation;
using SMS.Application.Commands.Identity;

namespace SMS.Application.Validators.Identity
{
    public class LinkStudentToUserCommandValidator : AbstractValidator<LinkStudentToUserCommand>
    {
        public LinkStudentToUserCommandValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}