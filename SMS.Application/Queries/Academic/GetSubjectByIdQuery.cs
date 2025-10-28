using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetSubjectByIdQuery : IRequest<SubjectDto?>
    {
        public int SubjectId { get; set; }
    }
}