using MediatR;

namespace SMS.Application.Commands.Communication
{
    public class StartDirectConversationCommand : IRequest<int>
    {
        public int UserAId { get; set; }
        public int UserBId { get; set; }
    }
}