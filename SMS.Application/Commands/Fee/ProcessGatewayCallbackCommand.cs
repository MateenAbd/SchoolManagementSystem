using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class ProcessGatewayCallbackCommand : IRequest<int> // returns receiptId (if success), or <=0 for failure
    {
        public PaymentCallbackDto Callback { get; set; } = new();
    }
}