using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetApplicationFeeSummaryQuery : IRequest<AdmissionFeeSummaryDto>
    {
        public string? AcademicYear { get; set; }
        public string? ClassAppliedFor { get; set; }
    }
}