using FluentValidation;
using SMS.Application.Commands.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Validators.Student
{
    public class AddStudentCommandValidator : AbstractValidator<AddStudentCommand>
    {
        public AddStudentCommandValidator()
        {
            RuleFor(x => x.StudentDto.FirstName).NotEmpty().MinimumLength(2);
            RuleFor(x => x.StudentDto.ClassName).NotEmpty();
            //RuleFor(x => x.StudentDto.AdmissionNo).NotEmpty(); will generate this in db if not proivided
        }
    }
}
