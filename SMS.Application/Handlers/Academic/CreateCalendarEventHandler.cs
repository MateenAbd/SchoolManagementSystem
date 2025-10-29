using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class CreateCalendarEventHandler : IRequestHandler<CreateCalendarEventCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public CreateCalendarEventHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper; 
        }

        public Task<int> Handle(CreateCalendarEventCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AcademicCalendarEvent>(request.Event);
            return _uow.AcademicRepository.CreateCalendarEventAsync(cancellationToken, entity);
        }
    }
}