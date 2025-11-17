using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Communication;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Communication
{
    public class StartDirectConversationHandler : IRequestHandler<StartDirectConversationCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public StartDirectConversationHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(StartDirectConversationCommand request, CancellationToken cancellationToken)
        {
            return _uow.CommunicationRepository.CreateOrGetDirectConversationAsync(cancellationToken, request.UserAId, request.UserBId);
        }
           
    }
}