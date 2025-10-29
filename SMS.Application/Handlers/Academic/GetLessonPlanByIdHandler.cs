using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetLessonPlanByIdHandler : IRequestHandler<GetLessonPlanByIdQuery, LessonPlanDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetLessonPlanByIdHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow; 
            _mapper = mapper;
        }

        public async Task<LessonPlanDto?> Handle(GetLessonPlanByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AcademicRepository.GetLessonPlanByIdAsync(cancellationToken, request.PlanId);
            return entity is null ? null : _mapper.Map<LessonPlanDto>(entity);
        }
    }
}