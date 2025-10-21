using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetMeritListQuery : IRequest<IEnumerable<AdmissionMeritItemDto>>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassAppliedFor { get; set; } = string.Empty;
    }
}