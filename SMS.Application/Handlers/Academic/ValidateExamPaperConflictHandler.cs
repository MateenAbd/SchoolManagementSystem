using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class ValidateExamPaperConflictHandler : IRequestHandler<ValidateExamPaperConflictQuery, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ValidateExamPaperConflictHandler(IUnitOfWork uow, IMapper mapper) 
        {
            _uow = uow; 
            _mapper = mapper;
        }

        public Task<int> Handle(ValidateExamPaperConflictQuery request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<ExamPaper>(request.Paper);
            return _uow.AcademicRepository.ValidateExamPaperConflictAsync(cancellationToken, entity);
        }
    }
}