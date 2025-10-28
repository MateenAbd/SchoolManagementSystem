using System;

namespace SMS.Core.Entities
{
    public class BiometricUserMap
    {
        public int MapId { get; set; }
        public int DeviceId { get; set; }
        public string ExternalUserId { get; set; } = string.Empty; // ID from device
        public string PersonType { get; set; } = "Student";        // Student or Staff
        public int? StudentId { get; set; }                        // required when PersonType=Student
        public int? UserId { get; set; }                           // required when PersonType=Staff
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
    }
}