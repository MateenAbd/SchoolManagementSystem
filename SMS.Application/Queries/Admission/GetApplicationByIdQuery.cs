using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetApplicationByIdQuery : IRequest<AdmissionApplicationDto?>
    {
        public int ApplicationId { get; set; }
    }
}