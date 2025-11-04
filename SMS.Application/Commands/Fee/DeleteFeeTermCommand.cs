using MediatR;

namespace SMS.Application.Commands.Fee
{
    public class DeleteFeeTermCommand : IRequest<int>
    {
        public int TermId { get; set; }
    }
}