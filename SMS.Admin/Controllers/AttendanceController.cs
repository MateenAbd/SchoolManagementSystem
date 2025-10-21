using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Commands.Attendance;
using SMS.Application.Dto;
using SMS.Application.Queries.Attendance;
using SMS.Core.Logger.Interfaces;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AttendanceController(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> MarkStudentAttendance([FromBody] MarkStudentAttendanceCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, attendanceId = id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "MarkStudentAttendance validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MarkStudentAttendance failed");
                return StatusCode(500, new { success = false, error = "Mark failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> BulkMarkClassAttendance([FromBody] BulkMarkClassAttendanceCommand command, CancellationToken token)
        {
            try
            {
                var count = await _mediator.Send(command, token);
                return Ok(new { success = true, count });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "BulkMarkClassAttendance validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "BulkMarkClassAttendance failed");
                return StatusCode(500, new { success = false, error = "Bulk mark failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UpdateStudentAttendanceStatus([FromBody] UpdateStudentAttendanceStatusCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateStudentAttendanceStatus validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateStudentAttendanceStatus failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteStudentAttendance([FromBody] DeleteStudentAttendanceCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteStudentAttendance failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassAttendanceByDate(DateTime attendanceDate, string className, string? section, string? subjectCode, int? periodNo, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetClassAttendanceByDateQuery
                {
                    AttendanceDate = attendanceDate,
                    ClassName = className,
                    Section = section,
                    SubjectCode = subjectCode,
                    PeriodNo = periodNo
                }, token);

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetClassAttendanceByDate failed");
                return Json(Array.Empty<StudentAttendanceDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentAttendanceRange(int studentId, DateTime fromDate, DateTime toDate, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetStudentAttendanceRangeQuery
                {
                    StudentId = studentId,
                    FromDate = fromDate,
                    ToDate = toDate
                }, token);

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentAttendanceRange failed");
                return Json(Array.Empty<StudentAttendanceDto>());
            }
        }


        [HttpPost]
        public async Task<IActionResult> ApplyStudentLeave([FromBody] ApplyStudentLeaveCommand command, CancellationToken token)
        {
            try
            {
                var leaveId = await _mediator.Send(command, token);
                return Ok(new { success = true, leaveId });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "ApplyStudentLeave validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ApplyStudentLeave failed");
                return StatusCode(500, new { success = false, error = "Apply failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> ApproveStudentLeave([FromBody] ApproveStudentLeaveCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "ApproveStudentLeave validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ApproveStudentLeave failed");
                return StatusCode(500, new { success = false, error = "Approve failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> RejectStudentLeave([FromBody] RejectStudentLeaveCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "RejectStudentLeave validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RejectStudentLeave failed");
                return StatusCode(500, new { success = false, error = "Reject failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> CancelStudentLeave([FromBody] CancelStudentLeaveCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CancelStudentLeave failed");
                return StatusCode(500, new { success = false, error = "Cancel failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentLeavesRange(int studentId, DateTime fromDate, DateTime toDate, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetStudentLeavesRangeQuery
                {
                    StudentId = studentId,
                    FromDate = fromDate,
                    ToDate = toDate
                }, token);

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentLeavesRange failed");
                return Json(Array.Empty<StudentLeaveRequestDto>());
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> GetPendingStudentLeaves(DateTime? fromDate, DateTime? toDate, string? className, string? section, string? status, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetPendingStudentLeavesQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    ClassName = className,
                    Section = section,
                    Status = status
                }, token);

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetPendingStudentLeaves failed");
                return Json(Array.Empty<StudentLeaveRequestDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> MarkStaffAttendance([FromBody] MarkStaffAttendanceCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, attendanceId = id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "MarkStaffAttendance validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MarkStaffAttendance failed");
                return StatusCode(500, new { success = false, error = "Mark failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStaffAttendanceStatus([FromBody] UpdateStaffAttendanceStatusCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateStaffAttendanceStatus validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateStaffAttendanceStatus failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteStaffAttendance([FromBody] DeleteStaffAttendanceCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteStaffAttendance failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> GetStaffAttendanceRange(int userId, DateTime fromDate, DateTime toDate, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetStaffAttendanceRangeQuery
                {
                    UserId = userId,
                    FromDate = fromDate,
                    ToDate = toDate
                }, token);

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStaffAttendanceRange failed");
                return Json(Array.Empty<StaffAttendanceDto>());
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> GetStaffDailyAttendance(DateTime attendanceDate, string? status, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetStaffDailyAttendanceQuery
                {
                    AttendanceDate = attendanceDate,
                    Status = status
                }, token);

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStaffDailyAttendance failed");
                return Json(Array.Empty<StaffAttendanceDto>());
            }
        }
    }
}