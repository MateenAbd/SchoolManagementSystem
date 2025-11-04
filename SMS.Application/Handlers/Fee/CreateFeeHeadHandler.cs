using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class CreateFeeHeadHandler : IRequestHandler<CreateFeeHeadCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public CreateFeeHeadHandler(IUnitOfWork uow, IMapper mapper) 
        { 
            _uow = uow;
            _mapper = mapper; 
        }

        public Task<int> Handle(CreateFeeHeadCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<FeeHead>(request.Head);
            return _uow.FeeRepository.CreateFeeHeadAsync(cancellationToken, entity);
        }
    }
}