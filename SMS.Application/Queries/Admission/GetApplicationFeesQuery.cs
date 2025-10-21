using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Admission
{
    public class GetApplicationFeesQuery : IRequest<IEnumerable<AdmissionFeePaymentDto>>
    {
        public int ApplicationId { get; set; }
    }
}