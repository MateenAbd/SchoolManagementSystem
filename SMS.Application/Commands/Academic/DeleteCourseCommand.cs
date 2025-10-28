using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteCourseCommand : IRequest<int>
    {
        public int CourseId { get; set; }
    }
}