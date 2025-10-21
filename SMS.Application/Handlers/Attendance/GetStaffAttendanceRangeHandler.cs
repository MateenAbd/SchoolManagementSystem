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
    public class GetStaffAttendanceRangeHandler : IRequestHandler<GetStaffAttendanceRangeQuery, IEnumerable<StaffAttendanceDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetStaffAttendanceRangeHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<StaffAttendanceDto>> Handle(GetStaffAttendanceRangeQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetStaffAttendanceRangeAsync(cancellationToken, request.UserId, request.FromDate.Date, request.ToDate.Date);
            return _mapper.Map<IEnumerable<StaffAttendanceDto>>(list);
        }
    }
}