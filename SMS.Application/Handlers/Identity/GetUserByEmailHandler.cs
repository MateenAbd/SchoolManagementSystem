using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Identity;

namespace SMS.Application.Handlers.Identity
{
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetUserByEmailHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _uow.UserRepository.GetUserByEmailAsync(cancellationToken, request.Email);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }
    }
}