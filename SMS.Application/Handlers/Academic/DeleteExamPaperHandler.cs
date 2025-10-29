using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteExamPaperHandler : IRequestHandler<DeleteExamPaperCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteExamPaperHandler(IUnitOfWork uow) {
            _uow = uow;
        }

        public Task<int> Handle(DeleteExamPaperCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteExamPaperAsync(cancellationToken, request.PaperId);
    }
}