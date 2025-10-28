using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class UpsertBiometricUserMapHandler : IRequestHandler<UpsertBiometricUserMapCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpsertBiometricUserMapHandler(IUnitOfWork uow, IMapper mapper) 
        { 
            _uow = uow;
            _mapper = mapper;
        }

        public Task<int> Handle(UpsertBiometricUserMapCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<BiometricUserMap>(request.Map);
            return _uow.AttendanceRepository.UpsertBiometricUserMapAsync(cancellationToken, entity);
        }
    }
}