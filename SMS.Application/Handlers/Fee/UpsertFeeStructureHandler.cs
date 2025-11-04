using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class UpsertFeeStructureHandler : IRequestHandler<UpsertFeeStructureCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpsertFeeStructureHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpsertFeeStructureCommand request, CancellationToken cancellationToken)
        {
            var header = _mapper.Map<FeeStructureHeader>(request.Structure);
            var details = request.Structure.Details?.Select(d => _mapper.Map<FeeStructureDetail>(d)) ?? Enumerable.Empty<FeeStructureDetail>();
            return _uow.FeeRepository.UpsertFeeStructureAsync(cancellationToken, header, details);
        }
    }
}