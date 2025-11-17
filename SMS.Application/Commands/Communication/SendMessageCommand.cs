using MediatR;

namespace SMS.Application.Commands.Communication
{
    public class SendMessageCommand : IRequest<int> //returns messageId
    {
        public int ConversationId { get; set; }
        public int SenderUserId { get; set; }
        public string ContentType { get; set; } = "text";
        public string Body { get; set; } = string.Empty;
    }
}