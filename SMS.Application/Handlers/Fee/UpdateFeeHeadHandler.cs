using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class UpdateFeeHeadHandler : IRequestHandler<UpdateFeeHeadCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpdateFeeHeadHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpdateFeeHeadCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<FeeHead>(request.Head);
            return _uow.FeeRepository.UpdateFeeHeadAsync(cancellationToken, entity);
        }
    }
}