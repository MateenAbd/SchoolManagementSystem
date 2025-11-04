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
    public class GetFeeStructureHeadersHandler : IRequestHandler<GetFeeStructureHeadersQuery, IEnumerable<FeeStructureDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeStructureHeadersHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<FeeStructureDto>> Handle(GetFeeStructureHeadersQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetFeeStructureHeadersAsync(
                cancellationToken, request.AcademicYear, request.ClassName, request.Section, request.TermId, request.IsActive);
            // Map headers only; Details can be fetched by ID if needed
            var result = new List<FeeStructureDto>();
            foreach (var h in list) result.Add(_mapper.Map<FeeStructureDto>(h));
            return result;
        }
    }
}