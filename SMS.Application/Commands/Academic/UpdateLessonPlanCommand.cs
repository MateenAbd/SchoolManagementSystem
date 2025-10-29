using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpdateLessonPlanCommand : IRequest<int>
    {
        public LessonPlanDto Plan { get; set; } = new();
    }
}