using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class VerifyApplicationDocumentCommand : IRequest<int>
    {
        public int DocumentId { get; set; }
        public int VerifiedByUserId { get; set; }
        public bool Verified { get; set; } = true;
    }
}