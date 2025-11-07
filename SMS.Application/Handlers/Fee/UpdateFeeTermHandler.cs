using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Fee
{
    public class UpdateFeeTermHandler : IRequestHandler<UpdateFeeTermCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateFeeTermHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public Task<int> Handle(UpdateFeeTermCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<FeeTerm>(request.Term);
            return _uow.FeeRepository.UpdateFeeTermAsync(cancellationToken, entity);
        }
    }
}