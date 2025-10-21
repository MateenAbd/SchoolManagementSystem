using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetPendingStudentLeavesQuery : IRequest<IEnumerable<StudentLeaveRequestDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public string? Status { get; set; } = "Pending";
    }
}