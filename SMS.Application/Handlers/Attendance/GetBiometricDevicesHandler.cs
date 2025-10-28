using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Attendance;

namespace SMS.Application.Handlers.Attendance
{
    public class GetBiometricDevicesHandler : IRequestHandler<GetBiometricDevicesQuery, IEnumerable<BiometricDeviceDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetBiometricDevicesHandler(IUnitOfWork uow, IMapper mapper) 
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BiometricDeviceDto>> Handle(GetBiometricDevicesQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetBiometricDevicesAsync(cancellationToken, request.IsActive);
            return _mapper.Map<IEnumerable<BiometricDeviceDto>>(list);
        }
    }
}