using System;

namespace SMS.Application.Dto
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public string ConversationType { get; set; } = "Direct"; //Direct/Group
        public int OtherUserId { get; set; }//for Direct chats:the counterpart
        public string OtherUserName { get; set; } = "";
        public string LastMessagePreview { get; set; } = "";
        public DateTime? LastMessageAtUtc { get; set; }
        public int UnreadCount { get; set; }
    }
}