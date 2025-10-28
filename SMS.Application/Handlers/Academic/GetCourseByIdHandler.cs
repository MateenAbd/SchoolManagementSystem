using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, CourseDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCourseByIdHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow; 
            _mapper = mapper; 
        }

        public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AcademicRepository.GetCourseByIdAsync(cancellationToken, request.CourseId);
            return entity is null ? null : _mapper.Map<CourseDto>(entity);
        }
    }
}