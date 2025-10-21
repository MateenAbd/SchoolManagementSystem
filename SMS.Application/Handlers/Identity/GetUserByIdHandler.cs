using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Identity;

namespace SMS.Application.Handlers.Identity
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _uow.UserRepository.GetUserByIdAsync(cancellationToken, request.UserId);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }
    }
}