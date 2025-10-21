using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class SetApplicationDocumentsVerifiedHandler : IRequestHandler<SetApplicationDocumentsVerifiedCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public SetApplicationDocumentsVerifiedHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(SetApplicationDocumentsVerifiedCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.SetApplicationDocumentsVerifiedAsync(cancellationToken, request.ApplicationId, request.DocumentsVerified);
    }
}