using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class GenerateStudentTermFeeHandler : IRequestHandler<GenerateStudentTermFeeCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public GenerateStudentTermFeeHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(GenerateStudentTermFeeCommand request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.GenerateStudentTermFeeAsync(cancellationToken, request.StudentId, request.AcademicYear, request.TermId);
    }
}