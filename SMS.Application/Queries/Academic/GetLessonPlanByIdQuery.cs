using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetLessonPlanByIdQuery : IRequest<LessonPlanDto?>
    {
        public int PlanId { get; set; }
    }
}