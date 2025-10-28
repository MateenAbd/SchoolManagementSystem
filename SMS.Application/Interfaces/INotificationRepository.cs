using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<AbsentStudentContact>> GetAbsentStudentContactsAsync(CancellationToken token, DateTime attendanceDate, string? className, string? section);
        Task<int> InsertNotificationLogAsync(CancellationToken token, NotificationLog log);
        Task<int> UpdateNotificationLogStatusAsync(CancellationToken token, int notificationId, string status, string? error, DateTime? sentAtUtc, int attemptIncrement);
        Task<IEnumerable<NotificationLog>> GetNotificationLogsAsync(CancellationToken token, DateTime? fromDate, DateTime? toDate, string? type, string? status, string? className, string? section, int? studentId);
    }
}