using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpdateSubjectCommand : IRequest<int>
    {
        public SubjectDto Subject { get; set; } = new();
    }
}