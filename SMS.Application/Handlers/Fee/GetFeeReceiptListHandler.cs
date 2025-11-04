using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetFeeReceiptListHandler : IRequestHandler<GetFeeReceiptListQuery, IEnumerable<FeeReceiptDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetFeeReceiptListHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<FeeReceiptDto>> Handle(GetFeeReceiptListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetFeeReceiptListAsync(
                cancellationToken, request.AcademicYear, request.StudentId, request.TermId, request.FromDate, request.ToDate, request.PaymentMode);
            return _mapper.Map<IEnumerable<FeeReceiptDto>>(list);
        }
    }
}