using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
    {
        public CreateExamCommandValidator()
        {
            RuleFor(x => x.Exam.AcademicYear).NotEmpty();
            RuleFor(x => x.Exam.ExamName).NotEmpty();
            RuleFor(x => x.Exam.ExamType).NotEmpty();
            RuleFor(x => x.Exam.ClassName).NotEmpty();
            RuleFor(x => x.Exam.EndDate).GreaterThanOrEqualTo(x => x.Exam.StartDate).When(x => x.Exam.EndDate.HasValue);
        }
    }
}