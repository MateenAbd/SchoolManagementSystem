using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Hubs;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Communication
{
    public class GetUserConversationIdsHandler : IRequestHandler<GetUserConversationIdsQuery, int[]>
    {
        private readonly IUnitOfWork _uow;
        public GetUserConversationIdsHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<int[]> Handle(GetUserConversationIdsQuery request, CancellationToken cancellationToken)
        {
            var ids = await _uow.CommunicationRepository.GetUserConversationIdsAsync(cancellationToken, request.UserId);
            return ids.ToArray();
        }
    }
}