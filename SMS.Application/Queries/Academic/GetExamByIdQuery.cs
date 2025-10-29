using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetExamByIdQuery : IRequest<ExamDto?>
    {
        public int ExamId { get; set; }
    }
}