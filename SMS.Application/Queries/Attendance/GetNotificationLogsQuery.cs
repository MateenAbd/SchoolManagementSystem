using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetNotificationLogsQuery : IRequest<IEnumerable<NotificationLogDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Type { get; set; }   // Email/SMS
        public string? Status { get; set; } // Pending/Sent/Failed
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int? StudentId { get; set; }
    }
}