using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpsertExamPaperCommand : IRequest<int>
    {
        public ExamPaperDto Paper { get; set; } = new();//returns >0 id or negative conflict code
    }
}