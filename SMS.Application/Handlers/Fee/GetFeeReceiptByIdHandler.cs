using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetFeeReceiptByIdHandler : IRequestHandler<GetFeeReceiptByIdQuery, FeeReceiptDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetFeeReceiptByIdHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<FeeReceiptDto?> Handle(GetFeeReceiptByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.FeeRepository.GetFeeReceiptByIdAsync(cancellationToken, request.ReceiptId);
            return entity is null ? null : _mapper.Map<FeeReceiptDto>(entity);
        }
    }
}