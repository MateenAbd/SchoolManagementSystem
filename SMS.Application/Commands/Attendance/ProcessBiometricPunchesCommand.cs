using System;
using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class ProcessBiometricPunchesCommand : IRequest<int>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}