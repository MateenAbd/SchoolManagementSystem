using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Commands.Admission;
using SMS.Application.Dto;
using SMS.Application.Queries.Admission;
using SMS.Core.Logger.Interfaces;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class AdmissionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IWebHostEnvironment _env;

        public AdmissionController(IMediator mediator, ILog logger, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        } 

        // Inquiries
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateInquiry([FromBody] AdmissionInquiryDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new CreateInquiryCommand { Inquiry = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "CreateInquiry validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateInquiry failed");
                return StatusCode(500, new { success = false, error = "Create failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateInquiry([FromBody] AdmissionInquiryDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateInquiryCommand { Inquiry = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateInquiry validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateInquiry failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateInquiryStatus([FromBody] UpdateInquiryStatusCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateInquiryStatus failed");
                return StatusCode(500, new { success = false, error = "Status update failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInquiryById(int inquiryId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetInquiryByIdQuery { InquiryId = inquiryId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetInquiryById failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInquiryList(string? academicYear, string? interestedClass, string? leadStatus, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetInquiryListQuery
                {
                    AcademicYear = academicYear,
                    InterestedClass = interestedClass,
                    LeadStatus = leadStatus
                }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetInquiryList failed");
                return Json(Array.Empty<AdmissionInquiryDto>());
            }
        }

        // Applications
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] AdmissionApplicationDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new CreateApplicationCommand { Application = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "CreateApplication validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateApplication failed");
                return StatusCode(500, new { success = false, error = "Create failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateApplication([FromBody] AdmissionApplicationDto dto, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(new UpdateApplicationCommand { Application = dto }, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "UpdateApplication validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateApplication failed");
                return StatusCode(500, new { success = false, error = "Update failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateApplicationStatus failed");
                return StatusCode(500, new { success = false, error = "Status update failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationById(int applicationId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetApplicationByIdQuery { ApplicationId = applicationId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetApplicationById failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationList(string? academicYear, string? classAppliedFor, string? status, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetApplicationListQuery
                {
                    AcademicYear = academicYear,
                    ClassAppliedFor = classAppliedFor,
                    Status = status
                }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetApplicationList failed");
                return Json(Array.Empty<AdmissionApplicationDto>());
            }
        }

        // Fees
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CollectApplicationFee([FromBody] CollectApplicationFeeCommand command, CancellationToken token)
        {
            try
            {
                var paymentId = await _mediator.Send(command, token);
                return Ok(new { success = true, paymentId });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "CollectApplicationFee validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CollectApplicationFee failed");
                return StatusCode(500, new { success = false, error = "Collection failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationFees(int applicationId, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetApplicationFeesQuery { ApplicationId = applicationId }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetApplicationFees failed");
                return Json(Array.Empty<AdmissionFeePaymentDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationFeeSummary(string? academicYear, string? classAppliedFor, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetApplicationFeeSummaryQuery
                {
                    AcademicYear = academicYear,
                    ClassAppliedFor = classAppliedFor
                }, token);
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetApplicationFeeSummary failed");
                return StatusCode(500);
            }
        }

        // Shortlist
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> GenerateShortlist([FromBody] GenerateShortlistCommand command, CancellationToken token)
        {
            try
            {
                var count = await _mediator.Send(command, token);
                return Ok(new { success = true, count });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "GenerateShortlist validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GenerateShortlist failed");
                return StatusCode(500, new { success = false, error = "Shortlist generation failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShortlist(string academicYear, string classAppliedFor, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetShortlistQuery { AcademicYear = academicYear, ClassAppliedFor = classAppliedFor }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetShortlist failed");
                return Json(Array.Empty<AdmissionShortlistItemDto>());
            }
        }

        // Merit list
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> GenerateMeritList([FromBody] GenerateMeritListCommand command, CancellationToken token)
        {
            try
            {
                var count = await _mediator.Send(command, token);
                return Ok(new { success = true, count });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "GenerateMeritList validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GenerateMeritList failed");
                return StatusCode(500, new { success = false, error = "Merit list generation failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMeritList(string academicYear, string classAppliedFor, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetMeritListQuery { AcademicYear = academicYear, ClassAppliedFor = classAppliedFor }, token);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetMeritList failed");
                return Json(Array.Empty<AdmissionMeritItemDto>());
            }
        }

        // Application Documents
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> UploadApplicationDocument([FromForm] int applicationId, [FromForm] string? description, IFormFile file, CancellationToken token)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
                var allowedExt = new[] { ".pdf", ".docx", ".doc", ".png", ".jpg", ".jpeg" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExt.Contains(ext)) return BadRequest("Unsupported file type.");

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "admissions", applicationId.ToString(), "docs");
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

                var webPath = $"/uploads/admissions/{applicationId}/docs/{safeName}";
                var id = await _mediator.Send(new AddApplicationDocumentCommand
                {
                    ApplicationId = applicationId,
                    FileName = safeName,
                    FilePath = webPath,
                    ContentType = file.ContentType,
                    Description = description
                }, token);

                return Ok(new { success = true, id, filePath = webPath });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UploadApplicationDocument failed");
                return StatusCode(500, new { success = false, error = "Upload failed" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationDocuments(int applicationId, CancellationToken token)
        {
            try
            {
                var docs = await _mediator.Send(new GetApplicationDocumentsQuery { ApplicationId = applicationId }, token);
                return Json(docs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetApplicationDocuments failed");
                return Json(Array.Empty<AdmissionApplicationDocumentDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> VerifyApplicationDocument([FromBody] VerifyApplicationDocumentCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "VerifyApplicationDocument validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "VerifyApplicationDocument failed");
                return StatusCode(500, new { success = false, error = "Verification failed" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetApplicationDocumentsVerified([FromBody] SetApplicationDocumentsVerifiedCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SetApplicationDocumentsVerified failed");
                return StatusCode(500, new { success = false, error = "Status update failed" });
            }
        }

        // Confirm Admission
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ConfirmAdmission([FromBody] ConfirmAdmissionCommand command, CancellationToken token)
        {
            try
            {
                var studentId = await _mediator.Send(command, token);
                return Ok(new { success = true, studentId });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "ConfirmAdmission validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ConfirmAdmission failed");
                return StatusCode(500, new { success = false, error = "Confirmation failed" });
            }
        }
    }
}