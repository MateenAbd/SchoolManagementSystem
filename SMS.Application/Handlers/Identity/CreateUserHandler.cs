using MediatR;
using SMS.Application.Commands.Identity;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Identity
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _hasher;

        public CreateUserHandler(IUnitOfWork uow, IPasswordHasher hasher)
        {
            _uow = uow;
            _hasher = hasher;
        }
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.IsActive,
                PasswordHash = _hasher.HashPassword(request.Password)
            };

            var id = await _uow.UserRepository.CreateUserAsync(cancellationToken, user);
            return id;
        }
    }
}
