using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteCalendarEventCommand : IRequest<int>
    {
        public int EventId { get; set; }
    }
}