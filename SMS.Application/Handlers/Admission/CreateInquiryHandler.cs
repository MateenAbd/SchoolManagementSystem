using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Admission;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Admission
{
    public class CreateInquiryHandler : IRequestHandler<CreateInquiryCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateInquiryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow; _mapper = mapper;
        }

        public Task<int> Handle(CreateInquiryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AdmissionInquiry>(request.Inquiry);
            return _uow.AdmissionRepository.CreateInquiryAsync(cancellationToken, entity);
        }
    }
}