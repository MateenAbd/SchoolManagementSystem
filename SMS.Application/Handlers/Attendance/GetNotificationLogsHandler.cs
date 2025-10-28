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
    public class GetNotificationLogsHandler : IRequestHandler<GetNotificationLogsQuery, IEnumerable<NotificationLogDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetNotificationLogsHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationLogDto>> Handle(GetNotificationLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _uow.NotificationRepository.GetNotificationLogsAsync(
                cancellationToken, request.FromDate, request.ToDate, request.Type, request.Status, request.ClassName, request.Section, request.StudentId);
            return _mapper.Map<IEnumerable<NotificationLogDto>>(logs);
        }
    }
}