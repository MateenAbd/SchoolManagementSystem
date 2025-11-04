using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class DeleteFeeStructureHandler : IRequestHandler<DeleteFeeStructureCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteFeeStructureHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(DeleteFeeStructureCommand request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.DeleteFeeStructureAsync(cancellationToken, request.StructureId);
    }
}