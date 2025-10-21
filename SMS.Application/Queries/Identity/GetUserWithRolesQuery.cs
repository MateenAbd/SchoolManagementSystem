using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Identity
{
    public class GetUserWithRolesQuery : IRequest<UserWithRolesDto?>
    {
        public int UserId { get; set; }
    }
}