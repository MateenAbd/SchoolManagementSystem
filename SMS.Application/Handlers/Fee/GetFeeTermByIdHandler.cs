using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetFeeTermByIdHandler : IRequestHandler<GetFeeTermByIdQuery, FeeTermDto?>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeTermByIdHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<FeeTermDto?> Handle(GetFeeTermByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.FeeRepository.GetFeeTermByIdAsync(cancellationToken, request.TermId);
            return entity is null ? null : _mapper.Map<FeeTermDto>(entity);
        }
    }
}