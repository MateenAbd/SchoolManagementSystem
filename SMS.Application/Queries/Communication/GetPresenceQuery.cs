using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Communication
{
    public class GetPresenceQuery : IRequest<PresenceDto?>
    {
        public int UserId { get; set; }
    }
}