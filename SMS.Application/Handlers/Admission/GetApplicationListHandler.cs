using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Admission;

namespace SMS.Application.Handlers.Admission
{
    public class GetApplicationListHandler : IRequestHandler<GetApplicationListQuery, IEnumerable<AdmissionApplicationDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetApplicationListHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow; _mapper = mapper;
        }

        public async Task<IEnumerable<AdmissionApplicationDto>> Handle(GetApplicationListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AdmissionRepository.GetApplicationListAsync(cancellationToken, request.AcademicYear, request.ClassAppliedFor, request.Status);
            return _mapper.Map<IEnumerable<AdmissionApplicationDto>>(list);
        }
    }
}