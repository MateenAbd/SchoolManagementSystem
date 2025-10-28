using System;

namespace SMS.Application.Dto
{
    public class BiometricUserMapDto
    {
        public int MapId { get; set; }
        public int DeviceId { get; set; }
        public string ExternalUserId { get; set; } = string.Empty;
        public string PersonType { get; set; } = "Student"; // Student/Staff
        public int? StudentId { get; set; }
        public int? UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
    }
}