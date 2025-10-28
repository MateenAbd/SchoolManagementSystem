using System;

namespace SMS.Application.Dto
{
    public class StudentAttendanceSummaryDto
    {
        public DateTime AttendanceDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
        public int Total { get; set; }
    }
}