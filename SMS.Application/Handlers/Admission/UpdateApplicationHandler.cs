using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Admission
{
    public class UpdateApplicationHandler : IRequestHandler<UpdateApplicationCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateApplicationHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public Task<int> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AdmissionApplication>(request.Application);
            return _uow.AdmissionRepository.UpdateApplicationAsync(cancellationToken, entity);
        }
    }
}