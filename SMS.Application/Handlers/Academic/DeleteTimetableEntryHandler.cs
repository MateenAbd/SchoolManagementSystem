using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteTimetableEntryHandler : IRequestHandler<DeleteTimetableEntryCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteTimetableEntryHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(DeleteTimetableEntryCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteTimetableEntryAsync(cancellationToken, request.TimetableId);
    }
}