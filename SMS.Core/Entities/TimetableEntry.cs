using System;

namespace SMS.Core.Entities
{
    public class TimetableEntry
    {
        public int TimetableId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public byte DayOfWeek { get; set; }//1=Mon ... 7=Sun
        public int? PeriodNo { get; set; }
        public TimeSpan? StartTime { get; set; } //optional if PeriodNo used
        public TimeSpan? EndTime { get; set; }
        public int SubjectId { get; set; }//FK to Subjects
        public int? CourseId { get; set; }//optional link to course
        public int? TeacherUserId { get; set; }//FK to Users
        public int? RoomId { get; set; }//FK to Classrooms
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}