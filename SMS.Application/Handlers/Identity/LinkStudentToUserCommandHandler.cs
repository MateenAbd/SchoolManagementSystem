using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Identity;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Identity
{
    public class LinkStudentToUserCommandHandler : IRequestHandler<LinkStudentToUserCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public LinkStudentToUserCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(LinkStudentToUserCommand request, CancellationToken cancellationToken)
        {
            return _uow.UserRepository.LinkStudentToUserAsync(cancellationToken, request.StudentId, request.UserId);
        }
    }
}