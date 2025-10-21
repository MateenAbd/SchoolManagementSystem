using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class VerifyApplicationDocumentHandler : IRequestHandler<VerifyApplicationDocumentCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public VerifyApplicationDocumentHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(VerifyApplicationDocumentCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.VerifyApplicationDocumentAsync(cancellationToken, request.DocumentId, request.VerifiedByUserId, request.Verified);
    }
}