using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class CollectStudentFeeCommandValidator : AbstractValidator<CollectStudentFeeCommand>
    {
        public CollectStudentFeeCommandValidator()
        {
            RuleFor(x => x.Request.StudentId).GreaterThan(0);
            RuleFor(x => x.Request.AcademicYear).NotEmpty();
            RuleFor(x => x.Request.TermId).GreaterThan(0);
            RuleFor(x => x.Request.PaymentMode).NotEmpty();
            RuleFor(x => x.Request.Items).NotEmpty();
            RuleForEach(x => x.Request.Items).ChildRules(i =>
            {
                i.RuleFor(y => y.HeadId).GreaterThan(0);
                i.RuleFor(y => y.Amount).GreaterThan(0);
            });
        }
    }
}