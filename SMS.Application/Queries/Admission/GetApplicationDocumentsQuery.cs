using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetApplicationDocumentsQuery : IRequest<IEnumerable<AdmissionApplicationDocumentDto>>
    {
        public int ApplicationId { get; set; }
    }
}