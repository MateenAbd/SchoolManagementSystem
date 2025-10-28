using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class UpdateClassroomHandler : IRequestHandler<UpdateClassroomCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpdateClassroomHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpdateClassroomCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Classroom>(request.Room);
            return _uow.AcademicRepository.UpdateClassroomAsync(cancellationToken, entity);
        }
    }
}