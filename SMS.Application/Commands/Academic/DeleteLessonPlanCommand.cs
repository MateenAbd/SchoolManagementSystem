using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class DeleteLessonPlanCommand : IRequest<int>
    {
        public int PlanId { get; set; }
    }
}