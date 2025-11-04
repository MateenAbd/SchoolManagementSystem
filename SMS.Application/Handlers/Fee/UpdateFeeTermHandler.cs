using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class DeleteFeeTermHandler : IRequestHandler<DeleteFeeTermCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteFeeTermHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(DeleteFeeTermCommand request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.DeleteFeeTermAsync(cancellationToken, request.TermId);
    }
}