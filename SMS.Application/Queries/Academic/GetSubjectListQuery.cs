using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetSubjectListQuery : IRequest<IEnumerable<SubjectDto>>
    {
        public bool? IsActive { get; set; }
    }
}