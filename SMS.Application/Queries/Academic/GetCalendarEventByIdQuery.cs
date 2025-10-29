using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetCalendarEventByIdQuery : IRequest<AcademicCalendarEventDto?>
    {
        public int EventId { get; set; }
    }
}