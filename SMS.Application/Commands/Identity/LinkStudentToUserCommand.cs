using MediatR;

namespace SMS.Application.Commands.Identity
{
    public class LinkStudentToUserCommand : IRequest<int>
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
    }
}