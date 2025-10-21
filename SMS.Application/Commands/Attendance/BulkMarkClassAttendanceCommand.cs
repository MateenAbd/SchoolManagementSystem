using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Attendance
{
    public class BulkMarkClassAttendanceCommand : IRequest<int>
    {
        public DateTime AttendanceDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public int? MarkedByUserId { get; set; }
        public string? SubjectCode { get; set; }
        public int? PeriodNo { get; set; }

        public List<ClassAttendanceBulkItemDto> Items { get; set; } = new();
    }
}