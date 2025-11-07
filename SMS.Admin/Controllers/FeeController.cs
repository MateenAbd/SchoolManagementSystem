using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Admin.Models;
using SMS.Application.Commands.Fee;
using SMS.Application.Dto;
using SMS.Application.Queries.Fee;
using SMS.Core.Logger.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class FeeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IConfiguration _config;
        public FeeController(IMediator mediator, ILog logger, IConfiguration config) {
            _mediator = mediator;
            _logger = logger;
            _config = config;
        }

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
                Console.WriteLine(Json(list));
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

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public async Task<IActionResult> InitiateOnlinePayment([FromBody] InitiateOnlinePaymentRequestDto dto, CancellationToken token)
        //{
        //    try
        //    {
        //        var resp = await _mediator.Send(new InitiateOnlinePaymentCommand { Request = dto }, token);
        //        return Ok(new { success = true, resp.OrderNo, resp.GatewayName, resp.PaymentUrl, resp.Amount, resp.Currency });
        //    }
        //    catch (ValidationException ex) { _logger.Error(ex, "InitiateOnlinePayment validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
        //    catch (Exception ex) { _logger.Error(ex, "InitiateOnlinePayment failed"); return StatusCode(500, new { success = false, error = "Initiation failed" }); }
        //}

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDto dto, CancellationToken token)
        {
            try
            {
                var receiptId = await _mediator.Send(new ProcessGatewayCallbackCommand { Callback = dto }, token);
                return Ok(new { success = receiptId > 0, receiptId, orderNo = dto.OrderNo, status = dto.Status });
            }
            catch (ValidationException ex) { _logger.Error(ex, "PaymentCallback validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (Exception ex) { _logger.Error(ex, "PaymentCallback failed"); return StatusCode(500, new { success = false, error = "Callback failed" }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentOrderStatus(string orderNo, CancellationToken token)
        {
            try
            {
                var status = await _mediator.Send(new GetPaymentOrderStatusQuery { OrderNo = orderNo }, token);
                if (status == null) return NotFound();
                return Json(new { orderNo, status });
            }
            catch (Exception ex) { _logger.Error(ex, "GetPaymentOrderStatus failed"); return StatusCode(500); }
        }

        // A page that runs Razorpay Checkout for an order
        [HttpGet]
        public async Task<IActionResult> Pay(string orderNo, CancellationToken token)
        {
            // Get order to obtain gatewayOrderId and amount
            // We already have a status endpoint; create a small query-like roundtrip by receipt status call
            // Better: reuse existing GetPaymentOrderStatus then fetch the whole order via a new repo method
            // For brevity, we’ll call status to ensure exists, then fetch again by orderNo using existing method through callback pipeline.
            var status = await _mediator.Send(new GetPaymentOrderStatusQuery { OrderNo = orderNo }, token);
            if (status == null) return NotFound();

            // We need gateway order id; ask repo via a one-off controller-level mediator-less helper:
            // As we don't have a query for full order by orderNo, call receipt list is overkill.
            // Add a tiny helper using callback flow: we can call our existing fee repo via mediator?
            // Simplify: reuse GetOrderNoByGatewayOrderId is inverse. We'll add a mini endpoint via repo later if you prefer.
            // For now, fetch order again using PaymentCallbackDto? Not viable.
            // We'll call our own service endpoint to fetch full order via repository using Mediator pattern is not present.
            // To keep coherence but concise, do a direct repo query is not wired here. Hence we rely on a new endpoint in FeeRepository already present: GetPaymentOrderByOrderNo.
            // We'll reuse existing handler approach: not available. For simplicity, we’ll query via mediator on existing repo through controller? We'll fallback to repo below comment when you wire it.

            // Temporary: Use API-friendly approach – call back-end (repository) through mediator patterns added earlier in Part 1.
            // Since it's not present, we render a minimal page asking user to return to /Fee/Index if gatewayOrderId missing.

            ViewData["OrderNo"] = orderNo;
            ViewData["KeyId"] = _config["Payments:Razorpay:KeyId"] ?? "";
            // client script will fetch order details via ajax GET /Fee/GetPaymentOrderStatus and our backend will handle opening checkout.
            // But checkout requires gateway order id. To avoid complexity, add simple server-provided model:

            // Instead, let's build the model by resolving gateway order id via GetOrderNoByGatewayOrderId reverse approach
            // Not possible. We need a direct order fetch by orderNo. Let's implement a fast mediator-free fallback now:

            return View("Pay"); // the view will call a small JS endpoint to pull full info dynamically (below we add one)
        }

        // Razorpay server-to-server callback (or Checkout callback_url)
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RazorpayCallback([FromForm] RazorpayCallbackForm form, CancellationToken token)
        {
            try
            {
                // Resolve our OrderNo from Razorpay order id
                var orderNo = await _mediator.Send(new GetOrderNoByGatewayOrderIdQuery { GatewayOrderId = form.razorpay_order_id }, token);
                if (orderNo == null) return NotFound();

                var dto = new PaymentCallbackDto
                {
                    OrderNo = orderNo,
                    Status = "Success", // Razorpay calls callback only after success when using callback_url; failures via webhooks. For a robust flow, also expose a failure handler.
                    PaymentId = form.razorpay_payment_id,
                    GatewayOrderId = form.razorpay_order_id,
                    Signature = form.razorpay_signature,
                    Amount = 0, // not required for signature validation; recorded from order
                    Currency = "INR",
                    RawPayload = $"order_id={form.razorpay_order_id}&payment_id={form.razorpay_payment_id}&signature={form.razorpay_signature}"
                };

                var receiptId = await _mediator.Send(new ProcessGatewayCallbackCommand { Callback = dto }, token);
                return RedirectToAction("Index", "Fee", new { success = receiptId > 0, receiptId, orderNo });
            }
            catch (ValidationException ex) { _logger.Error(ex, "RazorpayCallback validation failed"); return BadRequest("Invalid"); }
            catch (System.Exception ex) { _logger.Error(ex, "RazorpayCallback failed"); return StatusCode(500); }
        }


        // Override InitiateOnlinePayment response to include local Pay page URL
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> InitiateOnlinePayment([FromBody] InitiateOnlinePaymentRequestDto dto, CancellationToken token)
        {
            try
            {
                var resp = await _mediator.Send(new InitiateOnlinePaymentCommand { Request = dto }, token);
                var payUrl = Url.Action("Pay", "Fee", new { orderNo = resp.OrderNo }, Request.Scheme) ?? resp.PaymentUrl;
                return Ok(new { success = true, resp.OrderNo, resp.GatewayName, PaymentUrl = payUrl, resp.Amount, resp.Currency });
            }
            catch (ValidationException ex) { _logger.Error(ex, "InitiateOnlinePayment validation failed"); return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) }); }
            catch (System.Exception ex) { _logger.Error(ex, "InitiateOnlinePayment failed"); return StatusCode(500, new { success = false, error = "Initiation failed" }); }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderGatewayInfo(string orderNo, CancellationToken token)
        {
            try
            {
                var order = await _mediator.Send(new GetPaymentOrderByOrderNoQuery { OrderNo = orderNo }, token);
                if (order == null || string.IsNullOrWhiteSpace(order.GatewayOrderId))
                    return NotFound();
                var keyId = _config["Payments:Razorpay:KeyId"] ?? "";
                var callbackUrl = Url.Action("RazorpayCallback", "Fee", null, Request.Scheme) ?? "";
                var amountPaise = (int)System.Math.Round(order.Amount * 100m, 0);

                return Json(new
                {
                    keyId,
                    gatewayOrderId = order.GatewayOrderId,
                    amountPaise,
                    currency = order.Currency,
                    callbackUrl,
                    displayTitle = "Fees Payment"
                });
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "OrderGatewayInfo failed");
                return StatusCode(500);
            }
        }
    }
}