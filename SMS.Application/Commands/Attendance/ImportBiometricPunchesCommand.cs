using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Attendance
{
    public class ImportBiometricPunchesCommand : IRequest<int>
    {
        public List<BiometricPunchDto> Punches { get; set; } = new();
    }
}