
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Queries.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SMS.Application.Handlers.Student
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
    {
        private readonly SMS.Application.Interfaces.IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetStudentByIdHandler(SMS.Application.Interfaces.IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.StudentRepository.GetStudentByIdAsync(cancellationToken, request.StudentId);
            return _mapper.Map<StudentDto>(entity);
        }
    }
}
