using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Communication;

namespace SMS.Application.Handlers.Communication
{
    public class GetPresenceHandler : IRequestHandler<GetPresenceQuery, PresenceDto?>
    {
        private readonly IUnitOfWork _uow;
        public GetPresenceHandler(IUnitOfWork uow) 
        { 
            _uow = uow; 
        }

        public async Task<PresenceDto?> Handle(GetPresenceQuery request, CancellationToken cancellationToken)
        {
            var p = await _uow.CommunicationRepository.GetUserPresenceAsync(cancellationToken, request.UserId);
            if (p == null) return null;
            return new PresenceDto { UserId = p.UserId, IsOnline = p.IsOnline, LastSeenUtc = p.LastSeenUtc };
        }
    }
}