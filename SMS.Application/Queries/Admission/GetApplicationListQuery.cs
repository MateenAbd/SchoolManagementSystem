using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetApplicationListQuery : IRequest<IEnumerable<AdmissionApplicationDto>>
    {
        public string? AcademicYear { get; set; }
        public string? ClassAppliedFor { get; set; }
        public string? Status { get; set; }
    }
}