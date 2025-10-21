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
    public class GetStudentLeavesRangeHandler : IRequestHandler<GetStudentLeavesRangeQuery, IEnumerable<StudentLeaveRequestDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetStudentLeavesRangeHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentLeaveRequestDto>> Handle(GetStudentLeavesRangeQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetStudentLeavesRangeAsync(cancellationToken, request.StudentId, request.FromDate.Date, request.ToDate.Date);
            return _mapper.Map<IEnumerable<StudentLeaveRequestDto>>(list);
        }
    }
}