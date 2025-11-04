using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeReceiptItemsQuery : IRequest<IEnumerable<FeeReceiptItemDto>>
    {
        public int ReceiptId { get; set; }
    }
}