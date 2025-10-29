using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteLessonPlanHandler : IRequestHandler<DeleteLessonPlanCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteLessonPlanHandler(IUnitOfWork uow) { 
            _uow = uow; 
        }

        public Task<int> Handle(DeleteLessonPlanCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteLessonPlanAsync(cancellationToken, request.PlanId);
    }
}