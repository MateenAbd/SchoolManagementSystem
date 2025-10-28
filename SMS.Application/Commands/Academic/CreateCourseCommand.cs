using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class CreateCourseCommand : IRequest<int>
    {
        public CourseDto Course { get; set; } = new();
    }
}