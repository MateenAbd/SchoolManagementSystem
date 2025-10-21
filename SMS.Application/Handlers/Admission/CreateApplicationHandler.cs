using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Admission
{
    public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateApplicationHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public Task<int> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AdmissionApplication>(request.Application);
            return _uow.AdmissionRepository.CreateApplicationAsync(cancellationToken, entity);
        }
    }
}