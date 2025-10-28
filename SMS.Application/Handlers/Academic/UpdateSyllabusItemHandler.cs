using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class UpdateSyllabusItemHandler : IRequestHandler<UpdateSyllabusItemCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateSyllabusItemHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpdateSyllabusItemCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CourseSyllabus>(request.Item);
            return _uow.AcademicRepository.UpdateSyllabusItemAsync(cancellationToken, entity);
        }
    }
}