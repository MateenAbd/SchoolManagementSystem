using System;
using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class ApplyStudentLeaveCommand : IRequest<int>
    {
        public int StudentId { get; set; }
        public string LeaveType { get; set; } = "Sick";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Reason { get; set; }
        public int? AppliedByUserId { get; set; }
    }
}