using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class UpsertStudentScholarshipHandler : IRequestHandler<UpsertStudentScholarshipCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpsertStudentScholarshipHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(UpsertStudentScholarshipCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<StudentScholarship>(request.Scholarship);
            return _uow.FeeRepository.UpsertStudentScholarshipAsync(cancellationToken, entity);
        }
    }
}