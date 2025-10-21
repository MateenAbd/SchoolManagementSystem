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
    public class GetPendingStudentLeavesHandler : IRequestHandler<GetPendingStudentLeavesQuery, IEnumerable<StudentLeaveRequestDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPendingStudentLeavesHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentLeaveRequestDto>> Handle(GetPendingStudentLeavesQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetPendingStudentLeavesAsync(
                cancellationToken, request.FromDate?.Date, request.ToDate?.Date, request.ClassName, request.Section, request.Status);
            return _mapper.Map<IEnumerable<StudentLeaveRequestDto>>(list);
        }
    }
}