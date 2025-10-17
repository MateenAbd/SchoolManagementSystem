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
    public class DeleteStudentByIdCommandHandler : IRequestHandler<DeleteStudentByIdCommand, string>
    {
        private readonly IUnitOfWork _uow;

        public DeleteStudentByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<string> Handle(DeleteStudentByIdCommand request, CancellationToken cancellationToken)
        {
            var id = await _uow.StudentRepository.DeleteStudentByIdAsync(cancellationToken, request.StudentId);
            return "Deleted: Student ID: " + id.ToString();
        }
    }
}
