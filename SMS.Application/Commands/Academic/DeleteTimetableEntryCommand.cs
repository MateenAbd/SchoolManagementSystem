using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteTimetableEntryCommand : IRequest<int>
    {
        public int TimetableId { get; set; }
    }
}