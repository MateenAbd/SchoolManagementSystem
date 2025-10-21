using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Admission
{
    public class UpdateApplicationCommand : IRequest<int>
    {
        public AdmissionApplicationDto Application { get; set; } = new();
    }
}