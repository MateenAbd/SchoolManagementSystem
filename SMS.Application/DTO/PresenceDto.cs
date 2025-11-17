using System;

namespace SMS.Application.Dto
{
    public class PresenceDto
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenUtc { get; set; }
    }
}