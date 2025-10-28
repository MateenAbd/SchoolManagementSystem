using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetDailyStudentAttendanceSummaryQuery : IRequest<IEnumerable<StudentAttendanceSummaryDto>>
    {
        public DateTime AttendanceDate { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
    }
}