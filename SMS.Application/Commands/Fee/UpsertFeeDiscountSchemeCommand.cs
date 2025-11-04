using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class UpsertFeeDiscountSchemeCommand : IRequest<int>
    {
        public FeeDiscountSchemeDto Scheme { get; set; } = new();
    }
}