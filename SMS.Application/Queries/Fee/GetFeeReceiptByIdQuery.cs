using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeReceiptByIdQuery : IRequest<FeeReceiptDto?>
    {
        public int ReceiptId { get; set; }
    }
}