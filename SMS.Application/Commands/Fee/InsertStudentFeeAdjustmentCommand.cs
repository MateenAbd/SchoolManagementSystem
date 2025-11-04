using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class InsertStudentFeeAdjustmentCommand : IRequest<int>
    {
        public StudentFeeAdjustmentDto Adjustment { get; set; } = new();
    }
}