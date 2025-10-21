using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class GenerateMeritListHandler : IRequestHandler<GenerateMeritListCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public GenerateMeritListHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(GenerateMeritListCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.GenerateMeritListAsync(cancellationToken, request.AcademicYear, request.ClassAppliedFor, request.TopN);
    }
}