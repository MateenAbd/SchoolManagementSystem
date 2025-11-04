using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class CreateFeeTermCommand : IRequest<int>
    {
        public FeeTermDto Term { get; set; } = new();
    }
}