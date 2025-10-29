using MediatR;

namespace SMS.Application.Commands.Academic
{
    public class UpdateLessonPlanStatusCommand : IRequest<int>
    {
        public int PlanId { get; set; }
        public string Status { get; set; } = "Planned"; // Draft/Planned/Delivered
    }
}