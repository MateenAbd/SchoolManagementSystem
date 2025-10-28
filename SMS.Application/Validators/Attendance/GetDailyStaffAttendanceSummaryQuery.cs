using System;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetDailyStaffAttendanceSummaryQuery : IRequest<StaffAttendanceSummaryDto?>
    {
        public DateTime AttendanceDate { get; set; }
    }
}