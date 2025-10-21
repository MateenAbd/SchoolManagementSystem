using System;

namespace SMS.Core.Entities
{
    public class StudentAttendance
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime AttendanceDate { get; set; } //DATE in DB
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public string? SubjectCode { get; set; } //optional for subject-wise
        public int? PeriodNo { get; set; }       // optional for subject-wise
        public string Status { get; set; } = "Present"; //Present/Absent/Late/Excused
        public string? Remarks { get; set; }
        public int? MarkedByUserId { get; set; }
        public DateTime MarkedAtUtc { get; set; }
    }
}