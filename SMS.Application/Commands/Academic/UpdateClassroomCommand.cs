using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpdateClassroomCommand : IRequest<int>
    {
        public ClassroomDto Room { get; set; } = new();
    }
}