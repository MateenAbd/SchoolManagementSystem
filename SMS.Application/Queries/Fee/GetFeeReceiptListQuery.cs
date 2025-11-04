using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeReceiptListQuery : IRequest<IEnumerable<FeeReceiptDto>>
    {
        public string? AcademicYear { get; set; }
        public int? StudentId { get; set; }
        public int? TermId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? PaymentMode { get; set; }
    }
}