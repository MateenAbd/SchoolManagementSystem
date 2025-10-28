using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Academic;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Academic
{
    public class DeleteClassroomHandler : IRequestHandler<DeleteClassroomCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public DeleteClassroomHandler(IUnitOfWork uow) 
        {
            _uow = uow;
        }

        public Task<int> Handle(DeleteClassroomCommand request, CancellationToken cancellationToken) =>
            _uow.AcademicRepository.DeleteClassroomAsync(cancellationToken, request.RoomId);
    }
}