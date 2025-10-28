using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Attendance;

namespace SMS.Application.Handlers.Attendance
{
    public class GetDailyStaffAttendanceSummaryHandler : IRequestHandler<GetDailyStaffAttendanceSummaryQuery, StaffAttendanceSummaryDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetDailyStaffAttendanceSummaryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<StaffAttendanceSummaryDto?> Handle(GetDailyStaffAttendanceSummaryQuery request, CancellationToken cancellationToken)
        {
            var s = await _uow.AttendanceRepository.GetDailyStaffAttendanceSummaryAsync(cancellationToken, request.AttendanceDate.Date);
            return s is null ? null : _mapper.Map<StaffAttendanceSummaryDto>(s);
        }
    }
}