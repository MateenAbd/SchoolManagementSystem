using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteSyllabusItemCommand : IRequest<int>
    {
        public int SyllabusId { get; set; }
    }
}