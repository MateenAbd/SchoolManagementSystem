using System;
using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class AutoMarkApprovedLeavesCommand : IRequest<int>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IncludeStudents { get; set; } = true;
        public bool IncludeStaff { get; set; } = true;
    }
}