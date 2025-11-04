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
    public class GetFeeFineRulesHandler : IRequestHandler<GetFeeFineRulesQuery, IEnumerable<FeeFineRuleDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetFeeFineRulesHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<FeeFineRuleDto>> Handle(GetFeeFineRulesQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetFeeFineRulesAsync(cancellationToken, request.AcademicYear, request.ClassName, request.Section, request.TermId, request.IsActive);
            return _mapper.Map<IEnumerable<FeeFineRuleDto>>(list);
        }
    }
}