using System;

namespace SMS.Core.Entities
{
    public class NotificationLog
    {
        public int NotificationId { get; set; }
        public string Type { get; set; } = "Email"; //Email/SMS
        public string Recipient { get; set; } = string.Empty;//email or phone
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; //Pending/Sent/Failed
        public string? Error { get; set; }
        public DateTime RelatedDate { get; set; } //attendance date
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int? StudentId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public int AttemptCount { get; set; }
    }
}