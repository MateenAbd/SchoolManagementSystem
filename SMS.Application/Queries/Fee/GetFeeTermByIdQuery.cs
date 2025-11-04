using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeTermByIdQuery : IRequest<FeeTermDto?>
    {
        public int TermId { get; set; }
    }
}