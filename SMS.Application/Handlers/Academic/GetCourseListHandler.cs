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
    public class GetCourseListHandler : IRequestHandler<GetCourseListQuery, IEnumerable<CourseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCourseListHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> Handle(GetCourseListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetCourseListAsync(
                cancellationToken, request.AcademicYear, request.ClassName, request.SubjectId, request.IsActive);
            return _mapper.Map<IEnumerable<CourseDto>>(list);
        }
    }
}