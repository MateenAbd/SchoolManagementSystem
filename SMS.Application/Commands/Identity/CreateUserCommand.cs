using MediatR;

namespace SMS.Application.Commands.Identity
{
    public class CreateUserCommand : IRequest<int>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}