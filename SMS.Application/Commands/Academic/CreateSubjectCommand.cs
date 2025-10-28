using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class CreateSubjectCommand : IRequest<int>
    {
        public SubjectDto Subject { get; set; } = new();
    }
}