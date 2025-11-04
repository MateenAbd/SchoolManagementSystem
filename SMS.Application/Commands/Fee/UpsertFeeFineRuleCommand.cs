using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class UpsertFeeFineRuleCommand : IRequest<int>
    {
        public FeeFineRuleDto Rule { get; set; } = new();
    }
}