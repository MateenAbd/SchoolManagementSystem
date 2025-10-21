using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Admission;

namespace SMS.Application.Handlers.Admission
{
    public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, AdmissionApplicationDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetApplicationByIdHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<AdmissionApplicationDto?> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AdmissionRepository.GetApplicationByIdAsync(cancellationToken, request.ApplicationId);
            return entity is null ? null : _mapper.Map<AdmissionApplicationDto>(entity);
        }
    }
}