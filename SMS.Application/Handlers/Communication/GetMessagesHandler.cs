using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Communication;

namespace SMS.Application.Handlers.Communication
{
    public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetMessagesHandler(IUnitOfWork uow)
        { 
            _uow = uow;
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var ok = await _uow.CommunicationRepository.IsParticipantAsync(cancellationToken, request.ConversationId, request.UserId);
            if (!ok) return Enumerable.Empty<MessageDto>();

            var rows = await _uow.CommunicationRepository.GetMessagesAsync(cancellationToken, request.ConversationId, request.UserId, request.PageSize, request.BeforeMessageId);
            return rows.Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                ConversationId = m.ConversationId,
                SenderUserId = m.SenderUserId,
                ContentType = m.ContentType,
                Body = m.Body,
                SentAtUtc = m.SentAtUtc,
                IsRead = false // proc returns read info via receipts
            });
        }
    }
}