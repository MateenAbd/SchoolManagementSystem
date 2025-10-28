using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.Course.SubjectId).GreaterThan(0);
            RuleFor(x => x.Course.ClassName).NotEmpty();
            RuleFor(x => x.Course.AcademicYear).NotEmpty();
        }
    }
}