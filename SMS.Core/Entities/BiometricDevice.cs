using System;

namespace SMS.Core.Entities
{
    public class BiometricDevice
    {
        public int DeviceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string? Vendor { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime RegisteredAtUtc { get; set; }
    }
}