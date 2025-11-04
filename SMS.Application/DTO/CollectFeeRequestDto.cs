using System;
using System.Collections.Generic;

namespace SMS.Application.Dto
{
    public class CollectFeeRequestDto
    {
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public string PaymentMode { get; set; } = "Cash"; // Cash/Card/UPI/NetBanking/OnlineGateway/Cheque/DD
        public string? ReferenceNo { get; set; }
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;
        public int? ReceivedByUserId { get; set; }
        public List<FeeReceiptItemDto> Items { get; set; } = new();
    }
}