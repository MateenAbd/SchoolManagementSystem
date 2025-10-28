using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteSyllabusItemHandler : IRequestHandler<DeleteSyllabusItemCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteSyllabusItemHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(DeleteSyllabusItemCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteSyllabusItemAsync(cancellationToken, request.SyllabusId);
    }
}