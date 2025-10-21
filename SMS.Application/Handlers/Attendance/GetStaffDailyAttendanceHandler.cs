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
    public class GetStaffDailyAttendanceHandler : IRequestHandler<GetStaffDailyAttendanceQuery, IEnumerable<StaffAttendanceDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetStaffDailyAttendanceHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<StaffAttendanceDto>> Handle(GetStaffDailyAttendanceQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetStaffDailyAttendanceAsync(cancellationToken, request.AttendanceDate.Date, request.Status);
            return _mapper.Map<IEnumerable<StaffAttendanceDto>>(list);
        }
    }
}