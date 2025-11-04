using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetFeeStructureByIdHandler : IRequestHandler<GetFeeStructureByIdQuery, FeeStructureDto?>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeStructureByIdHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<FeeStructureDto?> Handle(GetFeeStructureByIdQuery request, CancellationToken cancellationToken)
        {
            var header = await _uow.FeeRepository.GetFeeStructureHeaderByIdAsync(cancellationToken, request.StructureId);
            if (header == null) return null;
            var details = await _uow.FeeRepository.GetFeeStructureDetailsAsync(cancellationToken, header.StructureId);
            var dto = _mapper.Map<FeeStructureDto>(header);
            dto.Details = details.Select(d => _mapper.Map<FeeStructureDetailDto>(d)).ToList();
            return dto;
        }
    }
}