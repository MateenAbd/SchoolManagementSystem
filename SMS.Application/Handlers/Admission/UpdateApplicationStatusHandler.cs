using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class UpdateApplicationStatusHandler : IRequestHandler<UpdateApplicationStatusCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public UpdateApplicationStatusHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.UpdateApplicationStatusAsync(cancellationToken, request.ApplicationId, request.Status);
    }
}