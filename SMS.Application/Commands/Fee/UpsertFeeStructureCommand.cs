using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class UpsertFeeStructureCommand : IRequest<int>
    {
        public FeeStructureDto Structure { get; set; } = new();
    }
}