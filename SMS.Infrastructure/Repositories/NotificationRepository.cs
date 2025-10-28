using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;

namespace SMS.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IRepository _db;
        private readonly ILog _logger;

        public NotificationRepository(IRepository db)
        {
            _db = db;
            _logger = new LogService();
        }

        public Task<IEnumerable<AbsentStudentContact>> GetAbsentStudentContactsAsync(CancellationToken token, DateTime attendanceDate, string? className, string? section)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceDate", ParameterValue = attendanceDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AbsentStudentContact>(token, "GetAbsentStudentContacts", p);
        }

        public async Task<int> InsertNotificationLogAsync(CancellationToken token, NotificationLog log)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@Type", ParameterValue = log.Type, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Recipient", ParameterValue = log.Recipient, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Subject", ParameterValue = log.Subject, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Body", ParameterValue = log.Body, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = log.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Error", ParameterValue = log.Error, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RelatedDate", ParameterValue = log.RelatedDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = log.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = log.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StudentId", ParameterValue = log.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "InsertNotificationLog", p);
        }

        public async Task<int> UpdateNotificationLogStatusAsync(CancellationToken token, int notificationId, string status, string? error, DateTime? sentAtUtc, int attemptIncrement)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@NotificationId", ParameterValue = notificationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Error", ParameterValue = error, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SentAtUtc", ParameterValue = sentAtUtc, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AttemptIncrement", ParameterValue = attemptIncrement, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateNotificationLogStatus", p);
        }

        public Task<IEnumerable<NotificationLog>> GetNotificationLogsAsync(CancellationToken token, DateTime? fromDate, DateTime? toDate, string? type, string? status, string? className, string? section, int? studentId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@FromDate", ParameterValue = fromDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Type", ParameterValue = type, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<NotificationLog>(token, "GetNotificationLogs", p);
        }
    }
}