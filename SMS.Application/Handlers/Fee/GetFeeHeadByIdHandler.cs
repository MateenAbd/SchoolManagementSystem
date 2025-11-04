using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetFeeHeadByIdHandler : IRequestHandler<GetFeeHeadByIdQuery, FeeHeadDto?>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeHeadByIdHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<FeeHeadDto?> Handle(GetFeeHeadByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.FeeRepository.GetFeeHeadByIdAsync(cancellationToken, request.HeadId);
            return entity is null ? null : _mapper.Map<FeeHeadDto>(entity);
        }
    }
}