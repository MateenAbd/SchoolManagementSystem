using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetClassroomListQuery : IRequest<IEnumerable<ClassroomDto>>
    {
        public bool? IsActive { get; set; }
    }
}