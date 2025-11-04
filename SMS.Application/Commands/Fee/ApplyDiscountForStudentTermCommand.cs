using MediatR;

namespace SMS.Application.Commands.Fee
{
    public class ApplyDiscountForStudentTermCommand : IRequest<int>
    {
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
        public int? SchemeId { get; set; }     // if null, use mode/value below
        public string? Mode { get; set; }      // Percent/Amount (optional when SchemeId provided)
        public decimal? Value { get; set; }
        public decimal? CapAmount { get; set; }
    }
}