using System;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Attendance
{
    public class MarkStudentAttendanceCommand : IRequest<int>
    {
        public int StudentId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public string Status { get; set; } = "Present";
        public string? Remarks { get; set; }
        public int? MarkedByUserId { get; set; }

        // Subject-wise
        public string? SubjectCode { get; set; }
        public int? PeriodNo { get; set; }
    }
}