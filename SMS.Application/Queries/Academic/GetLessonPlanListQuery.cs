using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetLessonPlanListQuery : IRequest<IEnumerable<LessonPlanDto>>
    {
        public string? AcademicYear { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; } //draft/planned/delivered
    }
}