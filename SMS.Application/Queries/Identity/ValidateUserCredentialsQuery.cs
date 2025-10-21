using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Identity
{
    public class ValidateUserCredentialsQuery : IRequest<UserWithRolesDto?>
    {
        public string UserNameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}