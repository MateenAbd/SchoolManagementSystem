using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface IAttendanceRepository
    {
        //student attendance
        Task<int> UpsertStudentAttendanceAsync(CancellationToken token, StudentAttendance attendance);
        Task<int> UpdateStudentAttendanceStatusAsync(CancellationToken token, int attendanceId, string status, string? remarks);
        Task<int> DeleteStudentAttendanceAsync(CancellationToken token, int attendanceId);

        Task<IEnumerable<StudentAttendance>> GetClassAttendanceByDateAsync(CancellationToken token, DateTime date, string className, string? section, string? subjectCode, int? periodNo);
        Task<IEnumerable<StudentAttendance>> GetStudentAttendanceRangeAsync(CancellationToken token, int studentId, DateTime fromDate, DateTime toDate);

        // Student leave
        Task<int> ApplyStudentLeaveAsync(CancellationToken token, StudentLeaveRequest leave);
        Task<int> ApproveStudentLeaveAsync(CancellationToken token, int leaveId, int approvedByUserId);
        Task<int> RejectStudentLeaveAsync(CancellationToken token, int leaveId, int approvedByUserId, string rejectionReason);
        Task<int> CancelStudentLeaveAsync(CancellationToken token, int leaveId);
        Task<IEnumerable<StudentLeaveRequest>> GetStudentLeavesRangeAsync(CancellationToken token, int studentId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<StudentLeaveRequest>> GetPendingStudentLeavesAsync(CancellationToken token, DateTime? fromDate, DateTime? toDate, string? className, string? section, string? status);

        // Staff attendance
        Task<int> UpsertStaffAttendanceAsync(CancellationToken token, StaffAttendance attendance);
        Task<int> UpdateStaffAttendanceStatusAsync(CancellationToken token, int attendanceId, string status, string? remarks, DateTime? inTime, DateTime? outTime);
        Task<int> DeleteStaffAttendanceAsync(CancellationToken token, int attendanceId);
        Task<IEnumerable<StaffAttendance>> GetStaffAttendanceRangeAsync(CancellationToken token, int userId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<StaffAttendance>> GetStaffDailyAttendanceAsync(CancellationToken token, DateTime attendanceDate, string? status);

        // Auto-mark approved leaves
        Task<int> AutoMarkStudentApprovedLeavesRangeAsync(CancellationToken token, DateTime fromDate, DateTime toDate);
        Task<int> AutoMarkStaffApprovedLeavesRangeAsync(CancellationToken token, DateTime fromDate, DateTime toDate);

        // Daily summaries
        Task<IEnumerable<StudentAttendanceSummary>> GetDailyStudentAttendanceSummaryAsync(CancellationToken token, DateTime attendanceDate, string? className, string? section);
        Task<StaffAttendanceSummary?> GetDailyStaffAttendanceSummaryAsync(CancellationToken token, DateTime attendanceDate);

        // Biometric devices and mapping
        Task<int> RegisterBiometricDeviceAsync(CancellationToken token, BiometricDevice device);
        Task<int> UpsertBiometricUserMapAsync(CancellationToken token, BiometricUserMap map);
        Task<IEnumerable<BiometricDevice>> GetBiometricDevicesAsync(CancellationToken token, bool? isActive);
        Task<IEnumerable<BiometricUserMap>> GetBiometricUserMapsAsync(CancellationToken token, int? deviceId, string? personType);

        // Raw punches import + processing
        Task<long> ImportRawPunchAsync(CancellationToken token, BiometricRawPunch punch);
        Task<int> ImportRawPunchesAsync(CancellationToken token, IEnumerable<BiometricRawPunch> punches);
        Task<int> ProcessBiometricPunchesAsync(CancellationToken token, DateTime fromDate, DateTime toDate);
    }
}