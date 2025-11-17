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
    public class GetConversationsHandler : IRequestHandler<GetConversationsQuery, IEnumerable<ConversationDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetConversationsHandler(IUnitOfWork uow)
        { 
            _uow = uow;
        }

        public async Task<IEnumerable<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            // Returns summarized conversation list for the user (sp will aggregate last message + unread)
            var list = await _uow.CommunicationRepository.GetConversationsForUserAsync(cancellationToken, request.UserId);
            // sp will return Conversation with some fields; we will map to DTO in repository sp (projection), keeping here simple:
            //to avoid adding another projection entity, we will do a small transform in repo sp result to Conversation typed; for now we assume last message fields returned via extra columns.
            //for simplicity, we expect repository to materialize conversational DTO-like columns and pack into ConversationDto with a helper sp
            // Here, to keep baseline, i will re-query messages when building UI; in practice, i have read we should return dto shape from sp.
            return list.Select(x => new ConversationDto
            {
                ConversationId = x.ConversationId,
                ConversationType = x.ConversationType,
                OtherUserId = 0,
                OtherUserName = "",
                LastMessagePreview = "",
                LastMessageAtUtc = null,
                UnreadCount = 0
            });
        }
    }
}