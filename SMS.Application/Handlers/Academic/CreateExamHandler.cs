using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class CreateExamHandler : IRequestHandler<CreateExamCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateExamHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow; 
            _mapper = mapper; 
        }

        public Task<int> Handle(CreateExamCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Exam>(request.Exam);
            return _uow.AcademicRepository.CreateExamAsync(cancellationToken, entity);
        }
    }
}