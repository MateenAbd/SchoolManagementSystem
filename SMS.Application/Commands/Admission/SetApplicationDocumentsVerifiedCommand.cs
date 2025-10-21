using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class SetApplicationDocumentsVerifiedCommand : IRequest<int>
    {
        public int ApplicationId { get; set; }
        public bool DocumentsVerified { get; set; } = true;
    }
}