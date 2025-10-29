using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteExamHandler : IRequestHandler<DeleteExamCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteExamHandler(IUnitOfWork uow) {
            _uow = uow;
        }

        public Task<int> Handle(DeleteExamCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteExamAsync(cancellationToken, request.ExamId);
    }
}