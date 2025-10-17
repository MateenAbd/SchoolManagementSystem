using MediatR;
using SMS.Application.Commands.Student;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Student
{
    public class AddStudentDocumentCommandHandler : IRequestHandler<AddStudentDocumentCommand, long>
    {
        private readonly IUnitOfWork _uow;

        public AddStudentDocumentCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<long> Handle(AddStudentDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = new StudentDocument
            {
                StudentId = request.StudentId,
                FileName = request.FileName,
                FilePath = request.FilePath,
                ContentType = request.ContentType,
                Description = request.Description
            };

            return await _uow.StudentRepository.AddStudentDocumentAsync(cancellationToken, entity);
        }
    }
}