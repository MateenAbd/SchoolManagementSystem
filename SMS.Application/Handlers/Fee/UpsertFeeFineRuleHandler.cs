using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class UpsertFeeFineRuleHandler : IRequestHandler<UpsertFeeFineRuleCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpsertFeeFineRuleHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpsertFeeFineRuleCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<FeeFineRule>(request.Rule);
            return _uow.FeeRepository.UpsertFeeFineRuleAsync(cancellationToken, entity);
        }
    }
}