using System;

namespace SMS.Core.Entities
{
    public class PaymentGatewayEvent
    {
        public int EventId { get; set; }
        public int OrderId { get; set; }
        public string EventType { get; set; } = string.Empty;//Initiated/Callback/Verify/Update/Info/Error
        public string Payload { get; set; } = string.Empty;// JSON
        public DateTime CreatedAtUtc { get; set; }
    }
}