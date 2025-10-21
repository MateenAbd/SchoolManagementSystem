using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SMS.Application.Interfaces;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;
using SMS.Core.Entities;
using SMS.Core.Interfaces;


namespace SMS.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly IRepository _db;
        private readonly ILog _logger;

        public AttendanceRepository(IRepository db)
        {
            _db = db;
            _logger = new LogService();
        }

        public async Task<int> UpsertStudentAttendanceAsync(CancellationToken token, StudentAttendance attendance)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = attendance.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AttendanceDate", ParameterValue = attendance.AttendanceDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ClassName", ParameterValue = attendance.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Section", ParameterValue = attendance.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@SubjectCode", ParameterValue = attendance.SubjectCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PeriodNo", ParameterValue = attendance.PeriodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Status", ParameterValue = attendance.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Remarks", ParameterValue = attendance.Remarks, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@MarkedByUserId", ParameterValue = attendance.MarkedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "UpsertStudentAttendance", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpsertStudentAttendance failed");
                throw;
            }
        }

        public async Task<int> UpdateStudentAttendanceStatusAsync(CancellationToken token, int attendanceId, string status, string? remarks)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceId", ParameterValue = attendanceId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Remarks", ParameterValue = remarks, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateStudentAttendanceStatus", p);
        }

        public async Task<int> DeleteStudentAttendanceAsync(CancellationToken token, int attendanceId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceId", ParameterValue = attendanceId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteStudentAttendance", p);
        }

        public Task<IEnumerable<StudentAttendance>> GetClassAttendanceByDateAsync(CancellationToken token, DateTime date, string className, string? section, string? subjectCode, int? periodNo)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceDate", ParameterValue = date.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectCode", ParameterValue = subjectCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PeriodNo", ParameterValue = periodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentAttendance>(token, "GetClassAttendanceByDate", p);
        }

        public Task<IEnumerable<StudentAttendance>> GetStudentAttendanceRangeAsync(CancellationToken token, int studentId, DateTime fromDate, DateTime toDate)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FromDate", ParameterValue = fromDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentAttendance>(token, "GetStudentAttendanceRange", p);
        }

        public async Task<int> ApplyStudentLeaveAsync(CancellationToken token, StudentLeaveRequest leave)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = leave.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@LeaveType", ParameterValue = leave.LeaveType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FromDate", ParameterValue = leave.FromDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ToDate", ParameterValue = leave.ToDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Reason", ParameterValue = leave.Reason, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AppliedByUserId", ParameterValue = leave.AppliedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "ApplyStudentLeaveRequest", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ApplyStudentLeave failed");
                throw;
            }
        }

        public async Task<int> ApproveStudentLeaveAsync(CancellationToken token, int leaveId, int approvedByUserId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@LeaveId", ParameterValue = leaveId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ApprovedByUserId", ParameterValue = approvedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "ApproveStudentLeaveRequest", p);
        }

        public async Task<int> RejectStudentLeaveAsync(CancellationToken token, int leaveId, int approvedByUserId, string rejectionReason)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@LeaveId", ParameterValue = leaveId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ApprovedByUserId", ParameterValue = approvedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RejectionReason", ParameterValue = rejectionReason, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "RejectStudentLeaveRequest", p);
        }

        public async Task<int> CancelStudentLeaveAsync(CancellationToken token, int leaveId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@LeaveId", ParameterValue = leaveId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CancelStudentLeaveRequest", p);
        }

        public Task<IEnumerable<StudentLeaveRequest>> GetStudentLeavesRangeAsync(CancellationToken token, int studentId, DateTime fromDate, DateTime toDate)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FromDate", ParameterValue = fromDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentLeaveRequest>(token, "GetStudentLeavesRange", p);
        }

        public Task<IEnumerable<StudentLeaveRequest>> GetPendingStudentLeavesAsync(CancellationToken token, DateTime? fromDate, DateTime? toDate, string? className, string? section, string? status)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@FromDate", ParameterValue = fromDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentLeaveRequest>(token, "GetPendingStudentLeaves", p);
        }

        public async Task<int> UpsertStaffAttendanceAsync(CancellationToken token, StaffAttendance attendance)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@UserId", ParameterValue = attendance.UserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AttendanceDate", ParameterValue = attendance.AttendanceDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Status", ParameterValue = attendance.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@InTime", ParameterValue = attendance.InTime, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@OutTime", ParameterValue = attendance.OutTime, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Remarks", ParameterValue = attendance.Remarks, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@MarkedByUserId", ParameterValue = attendance.MarkedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Source", ParameterValue = attendance.Source, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "UpsertStaffAttendance", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpsertStaffAttendance failed");
                throw;
            }
        }

        public async Task<int> UpdateStaffAttendanceStatusAsync(CancellationToken token, int attendanceId, string status, string? remarks, DateTime? inTime, DateTime? outTime)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceId", ParameterValue = attendanceId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Remarks", ParameterValue = remarks, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@InTime", ParameterValue = inTime, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@OutTime", ParameterValue = outTime, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateStaffAttendanceStatus", p);
        }

        public async Task<int> DeleteStaffAttendanceAsync(CancellationToken token, int attendanceId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceId", ParameterValue = attendanceId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteStaffAttendance", p);
        }

        public Task<IEnumerable<StaffAttendance>> GetStaffAttendanceRangeAsync(CancellationToken token, int userId, DateTime fromDate, DateTime toDate)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FromDate", ParameterValue = fromDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StaffAttendance>(token, "GetStaffAttendanceRange", p);
        }

        public Task<IEnumerable<StaffAttendance>> GetStaffDailyAttendanceAsync(CancellationToken token, DateTime attendanceDate, string? status)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AttendanceDate", ParameterValue = attendanceDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StaffAttendance>(token, "GetStaffDailyAttendance", p);
        }
    }
}