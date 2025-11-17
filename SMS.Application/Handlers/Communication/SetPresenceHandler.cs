using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Communication;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Communication
{
    public class SetPresenceHandler : IRequestHandler<SetPresenceCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public SetPresenceHandler(IUnitOfWork uow) 
        {
            _uow = uow;
        }

        public Task<int> Handle(SetPresenceCommand request, CancellationToken cancellationToken)
        {
            return _uow.CommunicationRepository.SetUserPresenceAsync(cancellationToken, request.UserId, request.IsOnline, request.AtUtc);
        }
            
    }
}