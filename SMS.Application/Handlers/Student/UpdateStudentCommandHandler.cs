using AutoMapper;
using MediatR;
using SMS.Application.Interfaces;
using SMS.Application.Commands.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Student
{
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, string>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateStudentCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<SMS.Core.Entities.Student>(request.StudentDto);
            var id = await _uow.StudentRepository.UpdateStudentAsync(cancellationToken, entity);
            return "OK : Student ID = " + id.ToString();
        }
    }
}
