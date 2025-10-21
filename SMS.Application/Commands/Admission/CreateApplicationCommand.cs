using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Admission
{
    public class CreateApplicationCommand : IRequest<int>
    {
        public AdmissionApplicationDto Application { get; set; } = new();
    }
}