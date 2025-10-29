using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpdateCalendarEventCommand : IRequest<int>
    {
        public AcademicCalendarEventDto Event { get; set; } = new();
    }
}