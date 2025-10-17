using MediatR;
using SMS.Application.Commands.Student;
using SMS.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Application.Handlers.Student
{
    public class SetStudentPhotoCommandHandler : IRequestHandler<SetStudentPhotoCommand, long>
    {
        private readonly IUnitOfWork _uow;

        public SetStudentPhotoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<long> Handle(SetStudentPhotoCommand request, CancellationToken cancellationToken)
        {
            return await _uow.StudentRepository.SetStudentPhotoAsync(cancellationToken, request.StudentId, request.PhotoUrl);
        }
    }
}