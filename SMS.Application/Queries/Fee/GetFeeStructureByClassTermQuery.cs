using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeStructureByClassTermQuery : IRequest<FeeStructureDto?>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public int TermId { get; set; }
    }
}