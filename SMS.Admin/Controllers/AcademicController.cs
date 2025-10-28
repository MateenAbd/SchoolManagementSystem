using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Commands.Academic;
using SMS.Application.Dto;
using SMS.Application.Queries.Academic;
using SMS.Core.Logger.Interfaces;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class AcademicController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AcademicController(IMediator mediator, ILog logger)
        {
            _mediator = mediator; _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() => View();

        // Subjects
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new CreateSubjectCommand { Subject = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "CreateSubject validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateSubject failed");
                return StatusCode(500, new { success = false, error = "Create failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateSubjectCommand { Subject = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateSubject validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateSubject failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteSubject([FromBody] int subjectId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteSubjectCommand { SubjectId = subjectId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteSubject failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectById(int subjectId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetSubjectByIdQuery { SubjectId = subjectId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetSubjectById failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectList(bool? isActive, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetSubjectListQuery { IsActive = isActive }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetSubjectList failed");
                return Json(Array.Empty<SubjectDto>());
            }
        }

        // Courses
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new CreateCourseCommand { Course = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "CreateCourse validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateCourse failed");
                return StatusCode(500, new { success = false, error = "Create failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateCourseCommand { Course = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateCourse validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateCourse failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourse([FromBody] int courseId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteCourseCommand { CourseId = courseId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteCourse failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseById(int courseId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetCourseByIdQuery { CourseId = courseId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetCourseById failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseList(string? academicYear, string? className, int? subjectId, bool? isActive, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetCourseListQuery
                {
                    AcademicYear = academicYear,
                    ClassName = className,
                    SubjectId = subjectId,
                    IsActive = isActive
                }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetCourseList failed");
                return Json(Array.Empty<CourseDto>());
            }
        }

        // Syllabus
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddSyllabusItem([FromBody] CourseSyllabusDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new AddSyllabusItemCommand { Item = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "AddSyllabusItem validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddSyllabusItem failed");
                return StatusCode(500, new { success = false, error = "Add failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UpdateSyllabusItem([FromBody] CourseSyllabusDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateSyllabusItemCommand { Item = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateSyllabusItem validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateSyllabusItem failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteSyllabusItem([FromBody] int syllabusId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteSyllabusItemCommand { SyllabusId = syllabusId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteSyllabusItem failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSyllabusByCourse(int courseId, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetSyllabusByCourseQuery { CourseId = courseId }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetSyllabusByCourse failed");
                return Json(Array.Empty<CourseSyllabusDto>());
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateClassroom([FromBody] ClassroomDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new CreateClassroomCommand { Room = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "CreateClassroom validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateClassroom failed");

                return StatusCode(500, new { success = false, error = "Create failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UpdateClassroom([FromBody] ClassroomDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateClassroomCommand { Room = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateClassroom validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateClassroom failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteClassroom([FromBody] int roomId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteClassroomCommand { RoomId = roomId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteClassroom failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassroomById(int roomId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetClassroomByIdQuery { RoomId = roomId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetClassroomById failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassroomList(bool? isActive, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetClassroomListQuery { IsActive = isActive }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetClassroomList failed");
                return Json(Array.Empty<ClassroomDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRoomsBySlot(string academicYear, byte dayOfWeek, int? periodNo, TimeSpan? startTime, TimeSpan? endTime, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetAvailableRoomsBySlotQuery
                {
                    AcademicYear = academicYear,
                    DayOfWeek = dayOfWeek,
                    PeriodNo = periodNo,
                    StartTime = startTime,
                    EndTime = endTime
                }, token);
                return Json(list);
            }
            catch (Exception ex) { _logger.Error(ex, "GetAvailableRoomsBySlot failed"); return Json(Array.Empty<ClassroomDto>()); }
        }


        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UpsertTimetableEntry([FromBody] TimetableEntryDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpsertTimetableEntryCommand { Entry = dto }, token);
                if (id < 0)
                {
                    var msg = id switch { -1 => "Class/Section conflict", -2 => "Teacher conflict", -3 => "Room conflict", _ => "Unknown conflict" };
                    return Conflict(new { success = false, error = msg, code = id });
                }
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex) { _logger.Error(ex, "UpsertTimetableEntry validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { 
                _logger.Error(ex, "UpsertTimetableEntry failed");
                return StatusCode(500, new { success = false, error = "Save failed" });
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteTimetableEntry([FromBody] int timetableId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteTimetableEntryCommand { TimetableId = timetableId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex) { 
                _logger.Error(ex, "DeleteTimetableEntry failed");
                return StatusCode(500, new { success = false, error = "Delete failed" }); 
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassTimetable(string academicYear, string className, string? section, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetClassTimetableQuery { AcademicYear = academicYear, ClassName = className, Section = section }, token);
                return Json(list);
            }
            catch (Exception ex) { 
                _logger.Error(ex, "GetClassTimetable failed"); 
                return Json(Array.Empty<TimetableEntryDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacherTimetable(string academicYear, int teacherUserId, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetTeacherTimetableQuery { AcademicYear = academicYear, TeacherUserId = teacherUserId }, token);
                return Json(list);
            }
            catch (Exception ex) {
                _logger.Error(ex, "GetTeacherTimetable failed");
                return Json(Array.Empty<TimetableEntryDto>());
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> ValidateTimetableConflict([FromBody] TimetableEntryDto dto, CancellationToken token)
        {
            try
            {
                var code = await _mediator.Send(new ValidateTimetableConflictQuery { Entry = dto }, token);
                return Json(new { code });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ValidateTimetableConflict failed");
                return StatusCode(500);
            }
        }
    }
}