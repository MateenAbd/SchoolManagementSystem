using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Attendance
{
    public class GetBiometricDevicesQuery : IRequest<IEnumerable<BiometricDeviceDto>>
    {
        public bool? IsActive { get; set; }
    }
}