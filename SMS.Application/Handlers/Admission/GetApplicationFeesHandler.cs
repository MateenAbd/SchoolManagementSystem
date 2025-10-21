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
    public class GetApplicationFeesHandler : IRequestHandler<GetApplicationFeesQuery, IEnumerable<AdmissionFeePaymentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetApplicationFeesHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdmissionFeePaymentDto>> Handle(GetApplicationFeesQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AdmissionRepository.GetApplicationFeesAsync(cancellationToken, request.ApplicationId);
            return _mapper.Map<IEnumerable<AdmissionFeePaymentDto>>(list);
        }
    }
}