using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetCourseListQuery : IRequest<IEnumerable<CourseDto>>
    {
        public string? AcademicYear { get; set; }
        public string? ClassName { get; set; }
        public int? SubjectId { get; set; }
        public bool? IsActive { get; set; }
    }
}