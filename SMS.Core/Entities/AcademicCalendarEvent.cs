using System;

namespace SMS.Core.Entities
{
    public class AcademicCalendarEvent
    {
        public int EventId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string EventType { get; set; } = "General";//Holiday/Exam/Activity/PTM/General
        public DateTime StartDate { get; set; }//date or datetime
        public DateTime? EndDate { get; set; }//inclusive; if null, same as StartDate
        public bool IsAllDay { get; set; } = true;

        public string? ClassName { get; set; }//optional scope
        public string? Section { get; set; }

        public string? Location { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}