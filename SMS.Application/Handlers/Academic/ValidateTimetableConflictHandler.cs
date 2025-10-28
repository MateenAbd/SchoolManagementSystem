using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class ValidateTimetableConflictHandler : IRequestHandler<ValidateTimetableConflictQuery, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public ValidateTimetableConflictHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(ValidateTimetableConflictQuery request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TimetableEntry>(request.Entry);
            return _uow.AcademicRepository.ValidateTimetableConflictAsync(cancellationToken, entity);
        }
    }
}