using System;

namespace SMS.Application.Dto
{
    public class TimetableEntryDto
    {
        public int TimetableId { get; set; }//0 for create, >0 for update
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public byte DayOfWeek { get; set; }//1..7
        public int? PeriodNo { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int SubjectId { get; set; }
        public int? CourseId { get; set; }
        public int? TeacherUserId { get; set; }
        public int? RoomId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}