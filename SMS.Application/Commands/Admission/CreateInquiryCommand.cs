using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Admission
{
    public class CreateInquiryCommand : IRequest<int>
    {
        public AdmissionInquiryDto Inquiry { get; set; } = new();
    }
}