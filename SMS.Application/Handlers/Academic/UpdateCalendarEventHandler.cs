using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class UpdateCalendarEventHandler : IRequestHandler<UpdateCalendarEventCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpdateCalendarEventHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow; 
            _mapper = mapper;
        }

        public Task<int> Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AcademicCalendarEvent>(request.Event);
            return _uow.AcademicRepository.UpdateCalendarEventAsync(cancellationToken, entity);
        }
    }
}