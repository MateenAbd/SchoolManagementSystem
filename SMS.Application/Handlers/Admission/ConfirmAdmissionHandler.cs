using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class ConfirmAdmissionHandler : IRequestHandler<ConfirmAdmissionCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ConfirmAdmissionHandler(IUnitOfWork uow)
        {
            _uow = uow; 
        }

        public Task<int> Handle(ConfirmAdmissionCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.ConfirmAdmissionAsync(cancellationToken, request.ApplicationId, request.Section, request.EnrollmentDate);
    }
}