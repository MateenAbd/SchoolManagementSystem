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
    public class GetDailyStudentAttendanceSummaryHandler : IRequestHandler<GetDailyStudentAttendanceSummaryQuery, IEnumerable<StudentAttendanceSummaryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetDailyStudentAttendanceSummaryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentAttendanceSummaryDto>> Handle(GetDailyStudentAttendanceSummaryQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetDailyStudentAttendanceSummaryAsync(
                cancellationToken, request.AttendanceDate.Date, request.ClassName, request.Section);
            return _mapper.Map<IEnumerable<StudentAttendanceSummaryDto>>(list);
        }
    }
}