using System;

namespace SMS.Application.Dto
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderUserId { get; set; }
        public string ContentType { get; set; } = "text";//text/file/image
        public string Body { get; set; } = string.Empty;
        public DateTime SentAtUtc { get; set; }
        public bool IsRead { get; set; }
    }
}