using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Admission;

namespace SMS.Application.Handlers.Admission
{
    public class GetApplicationFeeSummaryHandler : IRequestHandler<GetApplicationFeeSummaryQuery, AdmissionFeeSummaryDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetApplicationFeeSummaryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<AdmissionFeeSummaryDto> Handle(GetApplicationFeeSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = await _uow.AdmissionRepository.GetApplicationFeeSummaryAsync(cancellationToken, request.AcademicYear, request.ClassAppliedFor);
            return _mapper.Map<AdmissionFeeSummaryDto>(summary);
        }
    }
}