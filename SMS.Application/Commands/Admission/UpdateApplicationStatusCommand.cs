using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class UpdateApplicationStatusCommand : IRequest<int>
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } = "UnderReview";
    }
}