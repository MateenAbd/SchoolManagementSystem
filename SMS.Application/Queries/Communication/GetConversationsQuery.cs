using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Communication
{
    public class GetConversationsQuery : IRequest<IEnumerable<ConversationDto>>
    {
        public int UserId { get; set; }
    }
}