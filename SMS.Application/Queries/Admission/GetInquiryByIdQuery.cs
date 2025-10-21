using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetInquiryByIdQuery : IRequest<AdmissionInquiryDto?>
    {
        public int InquiryId { get; set; }
    }
}