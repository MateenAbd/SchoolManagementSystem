using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class CollectStudentFeeCommand : IRequest<int>
    {
        public CollectFeeRequestDto Request { get; set; } = new();
    }
}