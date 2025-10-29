using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetCalendarEventByIdHandler : IRequestHandler<GetCalendarEventByIdQuery, AcademicCalendarEventDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetCalendarEventByIdHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<AcademicCalendarEventDto?> Handle(GetCalendarEventByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AcademicRepository.GetCalendarEventByIdAsync(cancellationToken, request.EventId);
            return entity is null ? null : _mapper.Map<AcademicCalendarEventDto>(entity);
        }
    }
}