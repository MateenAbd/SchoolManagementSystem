using System;
using MediatR;

namespace SMS.Application.Commands.Attendance
{
    public class SendAbsenceAlertsCommand : IRequest<int>
    {
        public DateTime AttendanceDate { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public bool SendEmail { get; set; } = true;
        public bool SendSms { get; set; } = true;
    }
}