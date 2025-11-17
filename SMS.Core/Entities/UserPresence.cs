using System;

namespace SMS.Core.Entities
{
    public class UserPresence
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenUtc { get; set; }
        public DateTime? LastActiveUtc { get; set; }
    }
}