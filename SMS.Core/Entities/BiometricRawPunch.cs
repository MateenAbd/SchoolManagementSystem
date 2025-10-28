using System;

namespace SMS.Core.Entities
{
    public class BiometricRawPunch
    {
        public long PunchId { get; set; }
        public int DeviceId { get; set; }
        public string ExternalUserId { get; set; } = string.Empty;
        public DateTime PunchTime { get; set; }
        public string? Direction { get; set; }   // In/Out (optional)
        public string Source { get; set; } = "Biometric";
        public bool Processed { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}