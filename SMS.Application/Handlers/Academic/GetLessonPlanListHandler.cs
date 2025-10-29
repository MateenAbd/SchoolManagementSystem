using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetLessonPlanListHandler : IRequestHandler<GetLessonPlanListQuery, IEnumerable<LessonPlanDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetLessonPlanListHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper;
        }


        public async Task<IEnumerable<LessonPlanDto>> Handle(GetLessonPlanListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetLessonPlanListAsync(
                cancellationToken,
                request.AcademicYear, request.ClassName, request.Section,
                request.SubjectId, request.TeacherUserId,
                request.FromDate?.Date, request.ToDate?.Date, request.Status);
            return _mapper.Map<IEnumerable<LessonPlanDto>>(list);
        }
    }
}