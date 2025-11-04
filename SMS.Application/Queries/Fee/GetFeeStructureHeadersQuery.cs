using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeStructureHeadersQuery : IRequest<IEnumerable<FeeStructureDto>>
    {
        public string? AcademicYear { get; set; }
        public string? ClassName { get; set; }
        public string? Section { get; set; }
        public int? TermId { get; set; }
        public bool? IsActive { get; set; }
    }
}