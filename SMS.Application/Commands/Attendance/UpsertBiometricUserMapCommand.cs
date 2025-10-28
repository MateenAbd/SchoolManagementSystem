using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Attendance
{
    public class UpsertBiometricUserMapCommand : IRequest<int>
    {
        public BiometricUserMapDto Map { get; set; } = new();
    }
}