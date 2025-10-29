using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class ValidateExamPaperConflictQuery : IRequest<int>
    {
        public ExamPaperDto Paper { get; set; } = new();//returns 0 if ok, else negative code
    }
}