using FluentValidation;
using SMS.Application.Commands.Identity;

namespace SMS.Application.Validators.Identity
{
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.RoleName).NotEmpty().MinimumLength(3);
        }
    }
}