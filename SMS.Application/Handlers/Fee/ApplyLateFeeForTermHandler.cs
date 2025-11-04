using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class ApplyLateFeeForTermHandler : IRequestHandler<ApplyLateFeeForTermCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ApplyLateFeeForTermHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(ApplyLateFeeForTermCommand request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.ApplyLateFeeForTermAsync(cancellationToken, request.AcademicYear, request.TermId, request.AsOfDate);
    }
}