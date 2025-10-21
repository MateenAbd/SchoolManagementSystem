using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetClassAttendanceByDateQuery : IRequest<IEnumerable<StudentAttendanceDto>>
    {
        public DateTime AttendanceDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public int? PeriodNo { get; set; }
    }
}