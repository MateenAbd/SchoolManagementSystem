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
    public class GetTeacherTimetableHandler : IRequestHandler<GetTeacherTimetableQuery, IEnumerable<TimetableEntryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetTeacherTimetableHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<TimetableEntryDto>> Handle(GetTeacherTimetableQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetTeacherTimetableAsync(
                cancellationToken, request.AcademicYear, request.TeacherUserId);
            return _mapper.Map<IEnumerable<TimetableEntryDto>>(list);
        }
    }
}