using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class UpdateLessonPlanHandler : IRequestHandler<UpdateLessonPlanCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpdateLessonPlanHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow;
            _mapper = mapper;
        }

        public Task<int> Handle(UpdateLessonPlanCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<LessonPlan>(request.Plan);
            return _uow.AcademicRepository.UpdateLessonPlanAsync(cancellationToken, entity);
        }
    }
}