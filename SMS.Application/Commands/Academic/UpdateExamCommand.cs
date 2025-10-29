using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpdateExamCommand : IRequest<int>
    {
        public ExamDto Exam { get; set; } = new();
    }
}