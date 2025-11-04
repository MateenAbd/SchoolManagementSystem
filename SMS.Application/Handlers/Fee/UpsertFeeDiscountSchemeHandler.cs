using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class UpsertFeeDiscountSchemeHandler : IRequestHandler<UpsertFeeDiscountSchemeCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpsertFeeDiscountSchemeHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpsertFeeDiscountSchemeCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<FeeDiscountScheme>(request.Scheme);
            return _uow.FeeRepository.UpsertFeeDiscountSchemeAsync(cancellationToken, entity);
        }
    }
}