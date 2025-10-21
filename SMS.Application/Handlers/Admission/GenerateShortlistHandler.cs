using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class GenerateShortlistHandler : IRequestHandler<GenerateShortlistCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public GenerateShortlistHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(GenerateShortlistCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.GenerateShortlistAsync(cancellationToken, request.AcademicYear, request.ClassAppliedFor, request.MinEntranceScore, request.TopN);
    }
}