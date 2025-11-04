using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetFeeTermListQuery : IRequest<IEnumerable<FeeTermDto>>
    {
        public string? AcademicYear { get; set; }
        public bool? IsActive { get; set; }
    }
}