using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateSubjectHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper; 
        }

        public Task<int> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Subject>(request.Subject);
            return _uow.AcademicRepository.UpdateSubjectAsync(cancellationToken, entity);
        }
    }
}