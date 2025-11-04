using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class UpdateFeeHeadCommand : IRequest<int>
    {
        public FeeHeadDto Head { get; set; } = new();
    }
}