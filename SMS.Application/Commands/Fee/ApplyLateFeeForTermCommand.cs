using System;
using MediatR;

namespace SMS.Application.Commands.Fee
{
    public class ApplyLateFeeForTermCommand : IRequest<int>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public DateTime AsOfDate { get; set; }
    }
}