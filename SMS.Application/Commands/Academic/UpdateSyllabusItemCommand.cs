using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpdateSyllabusItemCommand : IRequest<int>
    {
        public CourseSyllabusDto Item { get; set; } = new();
    }
}