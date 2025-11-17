using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Communication;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Communication
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public SendMessageHandler(IUnitOfWork uow) 
        { 
            _uow = uow;
        }

        public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            //ensuring sender is participant
            var isParticipant = await _uow.CommunicationRepository.IsParticipantAsync(cancellationToken, request.ConversationId, request.SenderUserId);
            if (!isParticipant) return -1;

            var msg = new Message
            {
                ConversationId = request.ConversationId,
                SenderUserId = request.SenderUserId,
                ContentType = request.ContentType,
                Body = request.Body,
                SentAtUtc = DateTime.UtcNow
            };
            var messageId = await _uow.CommunicationRepository.InsertMessageAsync(cancellationToken, msg);

            // Mark Delivered for all other participants (server-side baseline)
            // Client will send AckRead to flip read status.
            //will do it in sp InsertMessage
            return messageId;
        }
    }
}