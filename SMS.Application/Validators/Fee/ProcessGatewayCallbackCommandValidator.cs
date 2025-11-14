using FluentValidation;
using SMS.Application.Commands.Fee;

namespace SMS.Application.Validators.Fee
{
    public class ProcessGatewayCallbackCommandValidator : AbstractValidator<ProcessGatewayCallbackCommand>
    {
        public ProcessGatewayCallbackCommandValidator()
        {
            RuleFor(x => x.Callback.OrderNo).NotEmpty();
            RuleFor(x => x.Callback.Status).NotEmpty();
            RuleFor(x => x.Callback.Currency).NotEmpty();
        }
    }
}