using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class CreateCalendarEventCommand : IRequest<int>
    {
        public AcademicCalendarEventDto Event { get; set; } = new();
    }
}