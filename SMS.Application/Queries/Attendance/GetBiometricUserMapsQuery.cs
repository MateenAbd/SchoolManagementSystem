using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetBiometricUserMapsQuery : IRequest<IEnumerable<BiometricUserMapDto>>
    {
        public int? DeviceId { get; set; }
        public string? PersonType { get; set; } // Student/Staff
    }
}