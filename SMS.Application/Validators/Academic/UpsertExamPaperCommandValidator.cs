using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpsertExamPaperCommandValidator : AbstractValidator<UpsertExamPaperCommand>
    {
        public UpsertExamPaperCommandValidator()
        {
            RuleFor(x => x.Paper.ExamId).GreaterThan(0);
            RuleFor(x => x.Paper.SubjectId).GreaterThan(0);
            RuleFor(x => x.Paper.ExamDate).NotEmpty();
            RuleFor(x => x.Paper.StartTime).NotEmpty();
            RuleFor(x => x.Paper.EndTime).GreaterThan(x => x.Paper.StartTime);
            RuleFor(x => x.Paper.DurationMinutes).GreaterThan(0);
            RuleFor(x => x.Paper.MaxMarks).GreaterThan(0);
            RuleFor(x => x.Paper.PassingMarks).LessThanOrEqualTo(x => x.Paper.MaxMarks).When(x => x.Paper.PassingMarks.HasValue);
        }
    }
}