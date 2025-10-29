using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteExamCommand : IRequest<int>
    {
        public int ExamId { get; set; }
    }
}