using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class UpdateFeeTermCommand : IRequest<int>
    {
        public FeeTermDto Term { get; set; } = new();
    }
}