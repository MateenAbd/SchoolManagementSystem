using FluentValidation;
using SMS.Application.Commands.Student;

namespace SMS.Application.Validators.Student
{
    public class AddStudentDocumentCommandValidator : AbstractValidator<AddStudentDocumentCommand>
    {
        public AddStudentDocumentCommandValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.FileName).NotEmpty();
            RuleFor(x => x.FilePath).NotEmpty();
        }
    }
}