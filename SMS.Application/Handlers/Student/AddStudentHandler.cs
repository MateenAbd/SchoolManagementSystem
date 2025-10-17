using AutoMapper;
using MediatR;
using SMS.Application.Commands.Student;
using SMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Student
{
    public class AddStudentHandler : IRequestHandler<AddStudentCommand, string>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AddStudentHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<string> Handle(AddStudentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<SMS.Core.Entities.Student>(request.StudentDto);
            var id = await _uow.StudentRepository.AddStudentAsync(cancellationToken, entity);
            return "OK : Student ID = " + id.ToString();
        }
    }
}
