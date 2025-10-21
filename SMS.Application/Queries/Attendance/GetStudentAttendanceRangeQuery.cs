using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetStudentAttendanceRangeQuery : IRequest<IEnumerable<StudentAttendanceDto>>
    {
        public int StudentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}