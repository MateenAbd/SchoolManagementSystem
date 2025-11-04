using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeStructureByIdQuery : IRequest<FeeStructureDto?>
    {
        public int StructureId { get; set; }
    }
}