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
    public class DeleteStudentDocumentByIdCommandHandler : IRequestHandler<DeleteStudentDocumentByIdCommand, long>
    {
        public IUnitOfWork _uow;

        public DeleteStudentDocumentByIdCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<long> Handle(DeleteStudentDocumentByIdCommand request, CancellationToken cancellationToken)
        {
            return _uow.StudentRepository.DeleteStudentDocumentByIdAsync(cancellationToken, request.DocumentId);
        }
    }
}
