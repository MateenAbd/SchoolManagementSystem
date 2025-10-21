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
    public class GetMeritListHandler : IRequestHandler<GetMeritListQuery, IEnumerable<AdmissionMeritItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetMeritListHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdmissionMeritItemDto>> Handle(GetMeritListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AdmissionRepository.GetMeritListAsync(cancellationToken, request.AcademicYear, request.ClassAppliedFor);
            return _mapper.Map<IEnumerable<AdmissionMeritItemDto>>(list);
        }
    }
}