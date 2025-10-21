using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetStaffDailyAttendanceQuery : IRequest<IEnumerable<StaffAttendanceDto>>
    {
        public DateTime AttendanceDate { get; set; }
        public string? Status { get; set; } // optional filter
    }
}