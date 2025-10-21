using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Admission;

namespace SMS.Application.Handlers.Admission
{
    public class GetApplicationDocumentsHandler : IRequestHandler<GetApplicationDocumentsQuery, IEnumerable<AdmissionApplicationDocumentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetApplicationDocumentsHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdmissionApplicationDocumentDto>> Handle(GetApplicationDocumentsQuery request, CancellationToken cancellationToken)
        {
            var docs = await _uow.AdmissionRepository.GetApplicationDocumentsAsync(cancellationToken, request.ApplicationId);
            return _mapper.Map<IEnumerable<AdmissionApplicationDocumentDto>>(docs);
        }
    }
}