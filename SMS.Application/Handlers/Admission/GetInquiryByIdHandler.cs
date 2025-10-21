using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Admission;

namespace SMS.Application.Handlers.Admission
{
    public class GetInquiryByIdHandler : IRequestHandler<GetInquiryByIdQuery, AdmissionInquiryDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetInquiryByIdHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<AdmissionInquiryDto?> Handle(GetInquiryByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AdmissionRepository.GetInquiryByIdAsync(cancellationToken, request.InquiryId);
            return entity is null ? null : _mapper.Map<AdmissionInquiryDto>(entity);
        }
    }
}