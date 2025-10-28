using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteSubjectHandler : IRequestHandler<DeleteSubjectCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public DeleteSubjectHandler(IUnitOfWork uow) {
            _uow = uow; 
        }

        public Task<int> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteSubjectAsync(cancellationToken, request.SubjectId);
    }
}