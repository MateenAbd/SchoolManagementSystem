using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteExamPaperCommand : IRequest<int>
    {
        public int PaperId { get; set; }
    }
}