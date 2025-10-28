using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteCourseHandler(IUnitOfWork uow) { 
            _uow = uow;
        }

        public Task<int> Handle(DeleteCourseCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteCourseAsync(cancellationToken, request.CourseId);
    }
}