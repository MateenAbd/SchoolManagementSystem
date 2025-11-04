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
    public class GetFeeReceiptItemsHandler : IRequestHandler<GetFeeReceiptItemsQuery, IEnumerable<FeeReceiptItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetFeeReceiptItemsHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<FeeReceiptItemDto>> Handle(GetFeeReceiptItemsQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetFeeReceiptItemsAsync(cancellationToken, request.ReceiptId);
            return _mapper.Map<IEnumerable<FeeReceiptItemDto>>(list);
        }
    }
}