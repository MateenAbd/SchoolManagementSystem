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
    public class GetFeeTermListHandler : IRequestHandler<GetFeeTermListQuery, IEnumerable<FeeTermDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeTermListHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<FeeTermDto>> Handle(GetFeeTermListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetFeeTermListAsync(cancellationToken, request.AcademicYear, request.IsActive);
            return _mapper.Map<IEnumerable<FeeTermDto>>(list);
        }
    }
}