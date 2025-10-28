using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetCourseByIdQuery : IRequest<CourseDto?>
    {
        public int CourseId { get; set; }
    }
}