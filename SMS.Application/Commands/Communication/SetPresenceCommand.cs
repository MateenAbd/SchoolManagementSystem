using System;
using MediatR;

namespace SMS.Application.Commands.Communication
{
    public class SetPresenceCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime AtUtc { get; set; }
    }
}