using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetClassroomByIdQuery : IRequest<ClassroomDto?>
    {
        public int RoomId { get; set; }
    }
}