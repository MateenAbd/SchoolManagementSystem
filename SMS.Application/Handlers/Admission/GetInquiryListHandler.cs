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
    public class GetInquiryListHandler : IRequestHandler<GetInquiryListQuery, IEnumerable<AdmissionInquiryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetInquiryListHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdmissionInquiryDto>> Handle(GetInquiryListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AdmissionRepository.GetInquiryListAsync(cancellationToken, request.AcademicYear, request.InterestedClass, request.LeadStatus);
            return _mapper.Map<IEnumerable<AdmissionInquiryDto>>(list);
        }
    }
}