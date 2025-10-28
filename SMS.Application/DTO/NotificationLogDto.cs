using System;

namespace SMS.Application.Dto
{
    public class NotificationLogDto
    {
        public int NotificationId { get; set; }
        public string Type { get; set; } = "Email";
        public string Recipient { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? Error { get; set; }
        public DateTime RelatedDate { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int? StudentId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public int AttemptCount { get; set; }
    }
}