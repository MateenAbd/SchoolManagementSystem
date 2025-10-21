using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class UpdateInquiryStatusCommand : IRequest<int>
    {
        public int InquiryId { get; set; }
        public string LeadStatus { get; set; } = "Contacted";
    }
}