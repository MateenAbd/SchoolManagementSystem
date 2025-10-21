using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Identity;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Identity
{
    public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public RemoveRoleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        {
            return _uow.UserRepository.RemoveRoleFromUserAsync(cancellationToken, request.UserId, request.RoleName);

        }
    }
}