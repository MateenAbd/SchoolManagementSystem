using FluentValidation;
using SMS.Application.Commands.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Validators.Student
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator()
        {
            RuleFor(x => x.StudentDto.StudentId).GreaterThan(0);
            RuleFor(x => x.StudentDto.FirstName).NotEmpty().MinimumLength(2);
            RuleFor(x => x.StudentDto.ClassName).NotEmpty();
        }
    }
}
