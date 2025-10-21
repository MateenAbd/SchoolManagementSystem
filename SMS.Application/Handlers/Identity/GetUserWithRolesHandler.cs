using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Identity;

namespace SMS.Application.Handlers.Identity
{
    public class GetUserWithRolesHandler : IRequestHandler<GetUserWithRolesQuery, UserWithRolesDto?>
    {
        private readonly IUnitOfWork _uow;
        public GetUserWithRolesHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UserWithRolesDto?> Handle(GetUserWithRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _uow.UserRepository.GetUserByIdAsync(cancellationToken, request.UserId);
            if (user == null) return null;
            var roles = await _uow.UserRepository.GetUserRolesAsync(cancellationToken, request.UserId);
            return new UserWithRolesDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = roles
            };
        }
    }
}