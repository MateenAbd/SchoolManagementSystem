using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class DeleteFeeHeadHandler : IRequestHandler<DeleteFeeHeadCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteFeeHeadHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(DeleteFeeHeadCommand request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.DeleteFeeHeadAsync(cancellationToken, request.HeadId);
    }
}