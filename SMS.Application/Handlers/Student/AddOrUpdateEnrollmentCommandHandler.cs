using AutoMapper;
using MediatR;
using SMS.Application.Commands.Student;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Student
{
    public class AddOrUpdateEnrollmentCommandHandler : IRequestHandler<AddOrUpdateEnrollmentCommand, long>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AddOrUpdateEnrollmentCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<long> Handle(AddOrUpdateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<StudentEnrollment>(request.Enrollment);
            return await _uow.StudentRepository.AddOrUpdateEnrollmentAsync(cancellationToken, entity);
        }
    }
}