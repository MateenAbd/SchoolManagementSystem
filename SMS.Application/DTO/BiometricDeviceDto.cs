using System;

namespace SMS.Application.Dto
{
    public class BiometricDeviceDto
    {
        public int DeviceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string? Vendor { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisteredAtUtc { get; set; }
    }
}