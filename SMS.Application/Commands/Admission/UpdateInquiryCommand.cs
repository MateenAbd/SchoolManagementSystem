using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Admission
{
    public class UpdateInquiryCommand : IRequest<int>
    {
        public AdmissionInquiryDto Inquiry { get; set; } = new();
    }
}