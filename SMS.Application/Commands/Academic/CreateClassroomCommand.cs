using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class CreateClassroomCommand : IRequest<int>
    {
        public ClassroomDto Room { get; set; } = new();
    }
}