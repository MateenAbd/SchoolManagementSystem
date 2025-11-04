using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeHeadByIdQuery : IRequest<FeeHeadDto?>
    {
        public int HeadId { get; set; }
    }
}