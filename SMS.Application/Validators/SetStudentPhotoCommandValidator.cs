using FluentValidation;
using SMS.Application.Commands.Student;

namespace SMS.Application.Validators.Student
{
    public class SetStudentPhotoCommandValidator : AbstractValidator<SetStudentPhotoCommand>
    {
        public SetStudentPhotoCommandValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.PhotoUrl).NotEmpty();
        }
    }
}   