using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetCalendarEventsHandler : IRequestHandler<GetCalendarEventsQuery, IEnumerable<AcademicCalendarEventDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetCalendarEventsHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<AcademicCalendarEventDto>> Handle(GetCalendarEventsQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetCalendarEventsAsync(
                cancellationToken, request.AcademicYear, request.FromDate?.Date, request.ToDate?.Date,
                request.ClassName, request.Section, request.EventType, request.IsActive);
            return _mapper.Map<IEnumerable<AcademicCalendarEventDto>>(list);
        }
    }
}