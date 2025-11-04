using MediatR;

namespace SMS.Application.Commands.Fee
{
    public class DeleteFeeStructureCommand : IRequest<int>
    {
        public int StructureId { get; set; }
    }
}