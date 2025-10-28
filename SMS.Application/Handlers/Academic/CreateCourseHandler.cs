using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateCourseHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow;
            _mapper = mapper; 
        }

        public Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Course>(request.Course);
            return _uow.AcademicRepository.CreateCourseAsync(cancellationToken, entity);
        }
    }
}