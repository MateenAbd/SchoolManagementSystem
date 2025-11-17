using MediatR;

namespace SMS.Application.Commands.Communication
{
    public class AckReadCommand : IRequest<int>
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
    }
}