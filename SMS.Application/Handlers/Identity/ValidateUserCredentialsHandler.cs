using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Identity;
using SMS.Core.Interfaces;

namespace SMS.Application.Handlers.Identity
{
    public class ValidateUserCredentialsHandler : IRequestHandler<ValidateUserCredentialsQuery, UserWithRolesDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _hasher;

        public ValidateUserCredentialsHandler(IUnitOfWork uow, IPasswordHasher hasher)
        {
            _uow = uow;
            _hasher = hasher;
        }

        public async Task<UserWithRolesDto?> Handle(ValidateUserCredentialsQuery request, CancellationToken cancellationToken)
        {
            var input = request.UserNameOrEmail?.Trim() ?? string.Empty;
            var user = input.Contains("@")
                ? await _uow.UserRepository.GetUserByEmailAsync(cancellationToken, input)
                : await _uow.UserRepository.GetUserByUserNameAsync(cancellationToken, input);

            if (user == null || !user.IsActive) return null;

            var ok = _hasher.VerifyPassword(user.PasswordHash, request.Password);
            if (!ok) return null;

            await _uow.UserRepository.UpdateUserLastLoginAsync(cancellationToken, user.UserId);
            var roles = await _uow.UserRepository.GetUserRolesAsync(cancellationToken, user.UserId);

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