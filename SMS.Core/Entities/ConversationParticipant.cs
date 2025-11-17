using System;

namespace SMS.Core.Entities
{
    public class ConversationParticipant
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAtUtc { get; set; }
        public int? LastReadMessageId { get; set; }
        public DateTime? LastSeenUtc { get; set; }
        public bool IsMuted { get; set; }
    }
}