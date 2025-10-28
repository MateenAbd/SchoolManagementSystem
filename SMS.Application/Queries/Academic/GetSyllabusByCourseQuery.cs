using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetSyllabusByCourseQuery : IRequest<IEnumerable<CourseSyllabusDto>>
    {
        public int CourseId { get; set; }
    }
}