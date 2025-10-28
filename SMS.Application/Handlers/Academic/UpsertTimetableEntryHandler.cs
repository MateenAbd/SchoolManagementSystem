using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Academic
{
    public class UpsertTimetableEntryHandler : IRequestHandler<UpsertTimetableEntryCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpsertTimetableEntryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpsertTimetableEntryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TimetableEntry>(request.Entry);
            return _uow.AcademicRepository.UpsertTimetableEntryAsync(cancellationToken, entity);
        }
    }
}