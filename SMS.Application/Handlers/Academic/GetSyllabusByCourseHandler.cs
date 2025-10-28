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
    public class GetSyllabusByCourseHandler : IRequestHandler<GetSyllabusByCourseQuery, IEnumerable<CourseSyllabusDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSyllabusByCourseHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<CourseSyllabusDto>> Handle(GetSyllabusByCourseQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetSyllabusByCourseAsync(cancellationToken, request.CourseId);
            return _mapper.Map<IEnumerable<CourseSyllabusDto>>(list);
        }
    }
}