using MediatR;

namespace SMS.Application.Commands.Fee
{
    public class DeleteFeeHeadCommand : IRequest<int>
    {
        public int HeadId { get; set; }
    }
}