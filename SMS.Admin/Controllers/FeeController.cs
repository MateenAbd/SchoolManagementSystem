using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Commands.Fee;
using SMS.Application.Dto;
using SMS.Application.Queries.Fee;
using SMS.Core.Logger.Interfaces;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class FeeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        public FeeController(IMediator mediator, ILog logger) { _mediator = mediator; _logger = logger; }

        [HttpGet]
        public IActionResult Index() => View();

        // Fee Heads
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateFeeHead([FromBody] FeeHeadDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new CreateFeeHeadCommand { Head = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "CreateFeeHead validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "CreateFeeHead failed"); return StatusCode(500, new { success = false, error = "Create failed" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateFeeHead([FromBody] FeeHeadDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new UpdateFeeHeadCommand { Head = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "UpdateFeeHead validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "UpdateFeeHead failed"); return StatusCode(500, new { success = false, error = "Update failed" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteFeeHead([FromBody] int headId, CancellationToken token)
        {
            try { var id = await _mediator.Send(new DeleteFeeHeadCommand { HeadId = headId }, token); return Ok(new { success = true, id }); }
            catch (Exception ex) { _logger.Error(ex, "DeleteFeeHead failed"); return StatusCode(500, new { success = false, error = "Delete failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeHeadById(int headId, CancellationToken token)
        {
            try { var dto = await _mediator.Send(new GetFeeHeadByIdQuery { HeadId = headId }, token); if (dto == null) return NotFound(); return Json(dto); }
            catch (Exception ex) { _logger.Error(ex, "GetFeeHeadById failed"); return StatusCode(500); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeHeadList(bool? isActive, CancellationToken token)
        {
            try { var list = await _mediator.Send(new GetFeeHeadListQuery { IsActive = isActive }, token); return Json(list); }
            catch (Exception ex) { _logger.Error(ex, "GetFeeHeadList failed"); return Json(Array.Empty<FeeHeadDto>()); }
        }

        // Fee Terms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateFeeTerm([FromBody] FeeTermDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new CreateFeeTermCommand { Term = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "CreateFeeTerm validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "CreateFeeTerm failed"); return StatusCode(500, new { success = false, error = "Create failed" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateFeeTerm([FromBody] FeeTermDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new UpdateFeeTermCommand { Term = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "UpdateFeeTerm validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "UpdateFeeTerm failed"); return StatusCode(500, new { success = false, error = "Update failed" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteFeeTerm([FromBody] int termId, CancellationToken token)
        {
            try { var id = await _mediator.Send(new DeleteFeeTermCommand { TermId = termId }, token); return Ok(new { success = true, id }); }
            catch (Exception ex) { _logger.Error(ex, "DeleteFeeTerm failed"); return StatusCode(500, new { success = false, error = "Delete failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeTermById(int termId, CancellationToken token)
        {
            try { var dto = await _mediator.Send(new GetFeeTermByIdQuery { TermId = termId }, token); if (dto == null) return NotFound(); return Json(dto); }
            catch (Exception ex) { _logger.Error(ex, "GetFeeTermById failed"); return StatusCode(500); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeTermList(string? academicYear, bool? isActive, CancellationToken token)
        {
            try { var list = await _mediator.Send(new GetFeeTermListQuery { AcademicYear = academicYear, IsActive = isActive }, token); return Json(list); }
            catch (Exception ex) { _logger.Error(ex, "GetFeeTermList failed"); return Json(Array.Empty<FeeTermDto>()); }
        }

        // Fee Structure
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpsertFeeStructure([FromBody] FeeStructureDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new UpsertFeeStructureCommand { Structure = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "UpsertFeeStructure validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "UpsertFeeStructure failed"); return StatusCode(500, new { success = false, error = "Save failed" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteFeeStructure([FromBody] int structureId, CancellationToken token)
        {
            try { var id = await _mediator.Send(new DeleteFeeStructureCommand { StructureId = structureId }, token); return Ok(new { success = true, id }); }
            catch (Exception ex) { _logger.Error(ex, "DeleteFeeStructure failed"); return StatusCode(500, new { success = false, error = "Delete failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeStructureById(int structureId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetFeeStructureByIdQuery { StructureId = structureId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex) { _logger.Error(ex, "GetFeeStructureById failed"); return StatusCode(500); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeStructureByClassTerm(string academicYear, string className, string? section, int termId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetFeeStructureByClassTermQuery { AcademicYear = academicYear, ClassName = className, Section = section, TermId = termId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex) { _logger.Error(ex, "GetFeeStructureByClassTerm failed"); return StatusCode(500); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeStructureHeaders(string? academicYear, string? className, string? section, int? termId, bool? isActive, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetFeeStructureHeadersQuery { AcademicYear = academicYear, ClassName = className, Section = section, TermId = termId, IsActive = isActive }, token);
                return Json(list);
            }
            catch (Exception ex) { _logger.Error(ex, "GetFeeStructureHeaders failed"); return Json(Array.Empty<FeeStructureDto>()); }
        }

        // Generate fee demand for a student/term
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> GenerateStudentTermFee([FromBody] GenerateStudentTermFeeCommand command, CancellationToken token)
        {
            try
            {
                var count = await _mediator.Send(command, token);
                return Ok(new { success = true, posted = count });
            }
            catch (ValidationException ex) { _logger.Error(ex, "GenerateStudentTermFee validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "GenerateStudentTermFee failed"); return StatusCode(500, new { success = false, error = "Generate failed" }); }
        }

        // Collect fee and generate receipt
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CollectStudentFee([FromBody] CollectFeeRequestDto dto, CancellationToken token)
        {
            try
            {
                var receiptId = await _mediator.Send(new CollectStudentFeeCommand { Request = dto }, token);
                return Ok(new { success = true, receiptId });
            }
            catch (ValidationException ex) { _logger.Error(ex, "CollectStudentFee validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "CollectStudentFee failed"); return StatusCode(500, new { success = false, error = "Collection failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeReceiptById(int receiptId, CancellationToken token)
        {
            try
            {
                var receipt = await _mediator.Send(new GetFeeReceiptByIdQuery { ReceiptId = receiptId }, token);
                if (receipt == null) return NotFound();
                return Json(receipt);
            }
            catch (Exception ex) { _logger.Error(ex, "GetFeeReceiptById failed"); return StatusCode(500); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeReceiptItems(int receiptId, CancellationToken token)
        {
            try
            {
                var items = await _mediator.Send(new GetFeeReceiptItemsQuery { ReceiptId = receiptId }, token);
                return Json(items);
            }
            catch (Exception ex) { _logger.Error(ex, "GetFeeReceiptItems failed"); return Json(Array.Empty<FeeReceiptItemDto>()); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeReceiptList(string? academicYear, int? studentId, int? termId, DateTime? fromDate, DateTime? toDate, string? paymentMode, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetFeeReceiptListQuery
                {
                    AcademicYear = academicYear,
                    StudentId = studentId,
                    TermId = termId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PaymentMode = paymentMode
                }, token);
                return Json(list);
            }
            catch (Exception ex) { _logger.Error(ex, "GetFeeReceiptList failed"); return Json(Array.Empty<FeeReceiptDto>()); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentLedger(int studentId, string? academicYear, int? termId, CancellationToken token)
        {
            try
            {
                var list = await _mediator.Send(new GetStudentLedgerQuery { StudentId = studentId, AcademicYear = academicYear, TermId = termId }, token);
                return Json(list);
            }
            catch (Exception ex) { _logger.Error(ex, "GetStudentLedger failed"); return Json(Array.Empty<StudentFeeLedgerDto>()); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentFeeBalance(int studentId, string? academicYear, int? termId, CancellationToken token)
        {
            try
            {
                var dto = await _mediator.Send(new GetStudentFeeBalanceQuery { StudentId = studentId, AcademicYear = academicYear, TermId = termId }, token);
                if (dto == null) return NotFound();
                return Json(dto);
            }
            catch (Exception ex) { _logger.Error(ex, "GetStudentFeeBalance failed"); return StatusCode(500); }
        }

        // Fine rules
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpsertFeeFineRule([FromBody] FeeFineRuleDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new UpsertFeeFineRuleCommand { Rule = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "UpsertFeeFineRule validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "UpsertFeeFineRule failed"); return StatusCode(500, new { success = false, error = "Save failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeFineRules(string? academicYear, string? className, string? section, int? termId, bool? isActive, CancellationToken token)
        {
            try { var list = await _mediator.Send(new GetFeeFineRulesQuery { AcademicYear = academicYear, ClassName = className, Section = section, TermId = termId, IsActive = isActive }, token); return Json(list); }
            catch (Exception ex) { _logger.Error(ex, "GetFeeFineRules failed"); return Json(Array.Empty<FeeFineRuleDto>()); }
        }

        // Discount schemes
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpsertFeeDiscountScheme([FromBody] FeeDiscountSchemeDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new UpsertFeeDiscountSchemeCommand { Scheme = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "UpsertFeeDiscountScheme validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "UpsertFeeDiscountScheme failed"); return StatusCode(500, new { success = false, error = "Save failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeDiscountSchemes(string? academicYear, string? className, string? section, int? termId, bool? isActive, CancellationToken token)
        {
            try { var list = await _mediator.Send(new GetFeeDiscountSchemesQuery { AcademicYear = academicYear, ClassName = className, Section = section, TermId = termId, IsActive = isActive }, token); return Json(list); }
            catch (Exception ex) { _logger.Error(ex, "GetFeeDiscountSchemes failed"); return Json(Array.Empty<FeeDiscountSchemeDto>()); }
        }

        // Student scholarships
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpsertStudentScholarship([FromBody] StudentScholarshipDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new UpsertStudentScholarshipCommand { Scholarship = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "UpsertStudentScholarship validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "UpsertStudentScholarship failed"); return StatusCode(500, new { success = false, error = "Save failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentScholarships(int? studentId, string? academicYear, int? termId, bool? isActive, CancellationToken token)
        {
            try { var list = await _mediator.Send(new GetStudentScholarshipsQuery { StudentId = studentId, AcademicYear = academicYear, TermId = termId, IsActive = isActive }, token); return Json(list); }
            catch (Exception ex) { _logger.Error(ex, "GetStudentScholarships failed"); return Json(Array.Empty<StudentScholarshipDto>()); }
        }

        // Apply late fee / apply discount
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ApplyLateFeeForTerm([FromBody] ApplyLateFeeForTermCommand command, CancellationToken token)
        {
            try { var count = await _mediator.Send(command, token); return Ok(new { success = true, posted = count }); }
            catch (ValidationException ex) { _logger.Error(ex, "ApplyLateFeeForTerm validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "ApplyLateFeeForTerm failed"); return StatusCode(500, new { success = false, error = "Apply failed" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ApplyDiscountForStudentTerm([FromBody] ApplyDiscountForStudentTermCommand command, CancellationToken token)
        {
            try { var count = await _mediator.Send(command, token); return Ok(new { success = true, posted = count }); }
            catch (ValidationException ex) { _logger.Error(ex, "ApplyDiscountForStudentTerm validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "ApplyDiscountForStudentTerm failed"); return StatusCode(500, new { success = false, error = "Apply failed" }); }
        }

        // Manual adjustments
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> InsertStudentFeeAdjustment([FromBody] StudentFeeAdjustmentDto dto, CancellationToken token)
        {
            try { var id = await _mediator.Send(new InsertStudentFeeAdjustmentCommand { Adjustment = dto }, token); return Ok(new { success = true, id }); }
            catch (ValidationException ex) { _logger.Error(ex, "InsertStudentFeeAdjustment validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "InsertStudentFeeAdjustment failed"); return StatusCode(500, new { success = false, error = "Insert failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentFeeAdjustments(int? studentId, string? academicYear, int? termId, string? type, CancellationToken token)
        {
            try { var list = await _mediator.Send(new GetStudentFeeAdjustmentsQuery { StudentId = studentId, AcademicYear = academicYear, TermId = termId, Type = type }, token); return Json(list); }
            catch (Exception ex) { _logger.Error(ex, "GetStudentFeeAdjustments failed"); return Json(Array.Empty<StudentFeeAdjustmentDto>()); }
        }
    }
}