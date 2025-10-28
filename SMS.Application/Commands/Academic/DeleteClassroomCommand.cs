using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteClassroomCommand : IRequest<int>
    {
        public int RoomId { get; set; }
    }
}