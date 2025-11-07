using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class InitiateOnlinePaymentCommand : IRequest<InitiateOnlinePaymentResponseDto>
    {
        public InitiateOnlinePaymentRequestDto Request { get; set; } = new();
    }
}