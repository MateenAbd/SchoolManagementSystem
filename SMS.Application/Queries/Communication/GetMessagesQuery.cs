using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Communication
{
    public class GetMessagesQuery : IRequest<IEnumerable<MessageDto>>
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int PageSize { get; set; } = 50;
        public int? BeforeMessageId { get; set; }
    }
}