using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetCalendarEventsQuery : IRequest<IEnumerable<AcademicCalendarEventDto>>
    {
        public string? AcademicYear { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public string? EventType { get; set; } // Holiday/Exam/Activity/PTM/General
        public bool? IsActive { get; set; }
    }
}