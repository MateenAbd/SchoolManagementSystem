using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class CreateSubjectHandler : IRequestHandler<CreateSubjectCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateSubjectHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow; 
            _mapper = mapper; 
        }

        public Task<int> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Subject>(request.Subject);
            return _uow.AcademicRepository.CreateSubjectAsync(cancellationToken, entity);
        }
    }
}