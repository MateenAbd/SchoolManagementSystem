using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class UpsertStudentScholarshipCommandValidator : AbstractValidator<UpsertStudentScholarshipCommand>
    {
        public UpsertStudentScholarshipCommandValidator()
        {
            RuleFor(x => x.Scholarship.StudentId).GreaterThan(0);
            RuleFor(x => x.Scholarship.AcademicYear).NotEmpty();
            RuleFor(x => x.Scholarship.Mode).Must(m => m == "Percent" || m == "Amount");
            RuleFor(x => x.Scholarship.Value).GreaterThan(0);
            RuleFor(x => x.Scholarship.ScholarshipHeadId).GreaterThan(0);
        }
    }
}