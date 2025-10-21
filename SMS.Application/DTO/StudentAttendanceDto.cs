using System;

namespace SMS.Application.Dto
{
    public class StudentAttendanceDto
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public int? PeriodNo { get; set; }
        public string Status { get; set; } = "Present";
        public string? Remarks { get; set; }
        public int? MarkedByUserId { get; set; }
        public DateTime MarkedAtUtc { get; set; }
    }
}