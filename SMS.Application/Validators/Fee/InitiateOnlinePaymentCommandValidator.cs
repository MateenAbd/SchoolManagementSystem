using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class InitiateOnlinePaymentCommandValidator : AbstractValidator<InitiateOnlinePaymentCommand>
    {
        public InitiateOnlinePaymentCommandValidator()
        {
            RuleFor(x => x.Request.StudentId).GreaterThan(0);
            RuleFor(x => x.Request.AcademicYear).NotEmpty();
            RuleFor(x => x.Request.TermId).GreaterThan(0);
            RuleFor(x => x.Request.Currency).NotEmpty();
            RuleFor(x => x.Request.Items).NotEmpty();
            RuleForEach(x => x.Request.Items).ChildRules(i =>
            {
                i.RuleFor(y => y.HeadId).GreaterThan(0);
                i.RuleFor(y => y.Amount).GreaterThan(0);
            });
        }
    }
}