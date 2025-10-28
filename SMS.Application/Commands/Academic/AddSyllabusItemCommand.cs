using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class AddSyllabusItemCommand : IRequest<int>
    {
        public CourseSyllabusDto Item { get; set; } = new();
    }
}