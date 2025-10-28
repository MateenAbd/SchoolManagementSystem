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
    public class GetBiometricUserMapsHandler : IRequestHandler<GetBiometricUserMapsQuery, IEnumerable<BiometricUserMapDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetBiometricUserMapsHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<BiometricUserMapDto>> Handle(GetBiometricUserMapsQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AttendanceRepository.GetBiometricUserMapsAsync(cancellationToken, request.DeviceId, request.PersonType);
            return _mapper.Map<IEnumerable<BiometricUserMapDto>>(list);
        }
    }
}