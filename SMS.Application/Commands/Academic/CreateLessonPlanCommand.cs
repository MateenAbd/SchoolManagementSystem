using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class CreateLessonPlanCommand : IRequest<int>
    {
        public LessonPlanDto Plan { get; set; } = new();
    }
}