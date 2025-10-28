using System;

namespace SMS.Application.Dto
{
    public class BiometricPunchDto
    {
        public int DeviceId { get; set; }
        public string ExternalUserId { get; set; } = string.Empty;
        public DateTime PunchTime { get; set; }
        public string? Direction { get; set; } // In/Out (optional)
    }
}