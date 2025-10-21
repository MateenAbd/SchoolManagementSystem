using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Admission
{
    public class UpdateInquiryStatusHandler : IRequestHandler<UpdateInquiryStatusCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public UpdateInquiryStatusHandler(IUnitOfWork uow) => _uow = uow;

        public Task<int> Handle(UpdateInquiryStatusCommand request, CancellationToken cancellationToken) =>
            _uow.AdmissionRepository.UpdateInquiryStatusAsync(cancellationToken, request.InquiryId, request.LeadStatus);
    }
}