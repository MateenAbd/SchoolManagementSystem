using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class CreateFeeTermHandler : IRequestHandler<CreateFeeTermCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public CreateFeeTermHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(CreateFeeTermCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<FeeTerm>(request.Term);
            return _uow.FeeRepository.CreateFeeTermAsync(cancellationToken, entity);
        }
    }
}