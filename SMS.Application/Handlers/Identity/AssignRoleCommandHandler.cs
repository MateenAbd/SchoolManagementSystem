using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Identity;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Identity
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public AssignRoleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            return _uow.UserRepository.AssignRoleToUserAsync(cancellationToken, request.UserId, request.RoleName);

        }
    }
}