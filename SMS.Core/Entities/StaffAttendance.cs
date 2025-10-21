using System;

namespace SMS.Core.Entities
{
    public class StaffAttendance
    {
        public int AttendanceId { get; set; }
        public int UserId { get; set; }                 // Staff user (Teacher/Admin/Staff)
        public DateTime AttendanceDate { get; set; }    //DATE in DB
        public string Status { get; set; } = "Present"; // Present/Absent/Late/Excused
        public DateTime? InTime { get; set; }           //optional
        public DateTime? OutTime { get; set; }          //optional
        public string? Remarks { get; set; }
        public int? MarkedByUserId { get; set; }
        public DateTime MarkedAtUtc { get; set; }
        public string? Source { get; set; }             // Manual/Biometric/RFID
    }
}