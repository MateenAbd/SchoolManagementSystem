using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteSubjectCommand : IRequest<int>
    {
        public int SubjectId { get; set; }
    }
}