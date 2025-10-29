using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetExamListQuery : IRequest<IEnumerable<ExamDto>>
    {
        public string? AcademicYear { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public string? ExamType { get; set; }
        public bool? IsPublished { get; set; }
    }
}