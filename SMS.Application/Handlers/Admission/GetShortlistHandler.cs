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
    public class GetShortlistHandler : IRequestHandler<GetShortlistQuery, IEnumerable<AdmissionShortlistItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetShortlistHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdmissionShortlistItemDto>> Handle(GetShortlistQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AdmissionRepository.GetShortlistAsync(cancellationToken, request.AcademicYear, request.ClassAppliedFor);
            return _mapper.Map<IEnumerable<AdmissionShortlistItemDto>>(list);
        }
    }
}