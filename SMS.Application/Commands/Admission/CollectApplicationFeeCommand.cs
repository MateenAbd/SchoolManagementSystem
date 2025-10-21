using System;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Admission
{
    public class CollectApplicationFeeCommand : IRequest<int>
    {
        public int ApplicationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMode { get; set; } = "Cash";
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }
        public int? CollectedByUserId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}