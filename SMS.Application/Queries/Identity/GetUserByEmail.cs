using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Identity
{
    public class GetUserByEmailQuery : IRequest<UserDto?>
    {
        public string Email { get; set; } = string.Empty;
    }
}