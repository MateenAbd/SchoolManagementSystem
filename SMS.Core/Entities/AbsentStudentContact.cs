using System;

namespace SMS.Core.Entities
{
    public class AbsentStudentContact
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? GuardianName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}