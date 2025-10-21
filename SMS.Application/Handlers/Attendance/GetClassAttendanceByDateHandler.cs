using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Attendance;

namespace SMS.Application.Handlers.Attendance
{
    public class GetClassAttendanceByDateHandler : IRequestHandler<GetClassAttendanceByDateQuery, IEnumerable<StudentAttendanceDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetClassAttendanceByDateHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentAttendanceDto>> Handle(GetClassAttendanceByDateQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetClassAttendanceByDateAsync(
                cancellationToken, request.AttendanceDate.Date, request.ClassName, request.Section, request.SubjectCode, request.PeriodNo);
            return _mapper.Map<IEnumerable<StudentAttendanceDto>>(list);
        }
    }
}