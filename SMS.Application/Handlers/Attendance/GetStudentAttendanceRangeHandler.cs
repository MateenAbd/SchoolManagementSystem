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
    public class GetStudentAttendanceRangeHandler : IRequestHandler<GetStudentAttendanceRangeQuery, IEnumerable<StudentAttendanceDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetStudentAttendanceRangeHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentAttendanceDto>> Handle(GetStudentAttendanceRangeQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetStudentAttendanceRangeAsync(
                cancellationToken, request.StudentId, request.FromDate.Date, request.ToDate.Date);
            return _mapper.Map<IEnumerable<StudentAttendanceDto>>(list);
        }
    }
}