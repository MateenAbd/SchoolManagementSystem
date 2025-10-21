using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Admission
{
    public class AddApplicationDocumentHandler : IRequestHandler<AddApplicationDocumentCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public AddApplicationDocumentHandler(IUnitOfWork uow)
        {
            _uow = uow; 
        }

        public Task<int> Handle(AddApplicationDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = new AdmissionApplicationDocument
            {
                ApplicationId = request.ApplicationId,
                FileName = request.FileName,
                FilePath = request.FilePath,
                ContentType = request.ContentType,
                Description = request.Description
            };
            return _uow.AdmissionRepository.AddApplicationDocumentAsync(cancellationToken, entity);
        }
    }
}