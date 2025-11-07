using System;
using System.Collections.Generic;

namespace SMS.Application.Dto
{
    public class InitiateOnlinePaymentRequestDto
    {
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public string Currency { get; set; } = "INR";
        public string? ReturnUrl { get; set; }
        public string? CallbackUrl { get; set; }
        public List<FeeReceiptItemDto> Items { get; set; } = new(); // heads + amounts
    }
}