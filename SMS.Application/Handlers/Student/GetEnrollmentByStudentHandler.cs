using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Student;

namespace SMS.Application.Handlers.Student
{
    public class GetEnrollmentByStudentHandler : IRequestHandler<GetEnrollmentByStudentQuery, StudentEnrollmentDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetEnrollmentByStudentHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<StudentEnrollmentDto?> Handle(GetEnrollmentByStudentQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.StudentRepository.GetEnrollmentByStudentAsync(cancellationToken, request.StudentId);
            return entity is null ? null : _mapper.Map<StudentEnrollmentDto>(entity);
        }
    }
}