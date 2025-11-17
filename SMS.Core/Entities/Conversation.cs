using System;

namespace SMS.Core.Entities
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public string ConversationType { get; set; } = "Direct";//Direct/Group
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;
    }
}