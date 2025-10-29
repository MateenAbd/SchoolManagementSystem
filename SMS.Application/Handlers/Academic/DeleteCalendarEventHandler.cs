using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteCalendarEventHandler : IRequestHandler<DeleteCalendarEventCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteCalendarEventHandler(IUnitOfWork uow) { 
            _uow = uow;
        }

        public Task<int> Handle(DeleteCalendarEventCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteCalendarEventAsync(cancellationToken, request.EventId);
    }
}