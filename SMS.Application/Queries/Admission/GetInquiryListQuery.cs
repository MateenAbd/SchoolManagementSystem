using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetInquiryListQuery : IRequest<IEnumerable<AdmissionInquiryDto>>
    {
        public string? AcademicYear { get; set; }
        public string? InterestedClass { get; set; }
        public string? LeadStatus { get; set; }
    }
}