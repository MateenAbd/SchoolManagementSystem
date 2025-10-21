using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SMS.Application.Commands.Student;
using SMS.Application.Dto;
using SMS.Application.Queries.Student;
using SMS.Core.Logger.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ILog _logger;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public StudentController(ILog logger, IStringLocalizer<SharedResources> localizer, IMediator mediator, IWebHostEnvironment env)
        {
            _logger = logger;
            _localizer = localizer;
            _mediator = mediator;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentDto studentDto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new AddStudentCommand { StudentDto = studentDto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "Validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddStudent failed");
                return StatusCode(500, new { success = false, error = "Add failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentById(int studentId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetStudentByIdQuery { StudentId = studentId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentById failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentList(CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetStudentListQuery(), token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentList failed");
                return Json(Array.Empty<StudentDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentDto studentDto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateStudentCommand { StudentDto = studentDto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "Validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateStudent failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteStudentById([FromBody] int studentId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteStudentByIdCommand { StudentId = studentId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteStudentById failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequestSizeLimit(20_000_000)] // 20 MB
        public async Task<IActionResult> UploadPhoto([FromForm] int studentId, IFormFile file, CancellationToken token)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(file.ContentType)) return BadRequest("Unsupported image type.");

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "students", studentId.ToString(), "photo");
                Directory.CreateDirectory(uploadsRoot);

                var ext = Path.GetExtension(file.FileName);
                var safeName = $"photo_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                var fullPath = Path.Combine(uploadsRoot, safeName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, token);
                }

                var webPath = $"/uploads/students/{studentId}/photo/{safeName}";
                var result = await _mediator.Send(new SetStudentPhotoCommand { StudentId = studentId, PhotoUrl = webPath }, token);
                return Ok(new { success = true, photoUrl = webPath, id = result });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UploadPhoto failed");
                return StatusCode(500, new { success = false, error = "Upload failed" });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<IActionResult> UploadDocument([FromForm] int studentId, [FromForm] string? description, IFormFile file, CancellationToken token)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
                var allowedExt = new[] { ".pdf", ".docx", ".doc", ".png", ".jpg", ".jpeg" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExt.Contains(ext)) return BadRequest("Unsupported file type.");

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "students", studentId.ToString(), "docs");
                Directory.CreateDirectory(uploadsRoot);

                var baseName = Path.GetFileNameWithoutExtension(file.FileName);
                var safeBase = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
                if (string.IsNullOrWhiteSpace(safeBase)) safeBase = "doc";
                var safeName = $"{safeBase}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                var fullPath = Path.Combine(uploadsRoot, safeName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, token);
                }

                var webPath = $"/uploads/students/{studentId}/docs/{safeName}";
                var id = await _mediator.Send(new AddStudentDocumentCommand
                {
                    StudentId = studentId,
                    FileName = safeName,
                    FilePath = webPath,
                    ContentType = file.ContentType,
                    Description = description
                }, token);

                return Ok(new { success = true, id, filePath = webPath });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UploadDocument failed");
                return StatusCode(500, new { success = false, error = "Upload failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentDocuments(int studentId, CancellationToken token)
        {
            try
            {
                var docs = await _mediator.Send(new GetStudentDocumentsQuery { StudentId = studentId }, token);
                return Json(docs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentDocuments failed");
                return Json(Array.Empty<StudentDocumentDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteStudentDocumentById([FromBody] int documentId, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new DeleteStudentDocumentByIdCommand { DocumentId = documentId }, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteStudentDocumentById failed");
                return StatusCode(500, new { success = false, error = "Delete failed" });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateEnrollment([FromBody] StudentEnrollmentDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new AddOrUpdateEnrollmentCommand { Enrollment = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "Enrollment validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddOrUpdateEnrollment failed");
                return StatusCode(500, new { success = false, error = "Save failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollmentByStudent([FromQuery] int studentId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetEnrollmentByStudentQuery { StudentId = studentId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetEnrollmentByStudent failed");
                return StatusCode(500);
            }
        }
    }
}