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
    public class GetFeeHeadListHandler : IRequestHandler<GetFeeHeadListQuery, IEnumerable<FeeHeadDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeHeadListHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<FeeHeadDto>> Handle(GetFeeHeadListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetFeeHeadListAsync(cancellationToken, request.IsActive);
            return _mapper.Map<IEnumerable<FeeHeadDto>>(list);
        }
    }
}