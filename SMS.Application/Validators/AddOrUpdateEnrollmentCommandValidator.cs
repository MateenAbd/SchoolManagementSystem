using FluentValidation;
using SMS.Application.Commands.Student;

namespace SMS.Application.Validators.Student
{
    public class AddOrUpdateEnrollmentCommandValidator : AbstractValidator<AddOrUpdateEnrollmentCommand>
    {
        public AddOrUpdateEnrollmentCommandValidator()
        {
            RuleFor(x => x.Enrollment.StudentId).GreaterThan(0);
            RuleFor(x => x.Enrollment.ClassName).NotEmpty();
            RuleFor(x => x.Enrollment.AcademicYear).NotEmpty();
        }
    }
}