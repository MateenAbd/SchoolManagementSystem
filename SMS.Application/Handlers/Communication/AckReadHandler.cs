using MediatR;
using SMS.Application.Commands.Communication;
using SMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SMS.Application.Handlers.Communication
{
    public class AckReadHandler : IRequestHandler<AckReadCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public AckReadHandler(IUnitOfWork uow)
        { 
            _uow = uow;
        }

        public async Task<int> Handle(AckReadCommand request, CancellationToken cancellationToken)
        {
            var ok = await _uow.CommunicationRepository.IsParticipantAsync(cancellationToken, request.ConversationId, request.UserId);
            if (!ok) return -1;

            await _uow.CommunicationRepository.InsertOrUpdateReceiptAsync(cancellationToken, request.MessageId, request.UserId, "Read");
            //update participant last read pointer
            return await _uow.CommunicationRepository.UpdateParticipantLastReadAsync(cancellationToken, request.ConversationId, request.UserId, request.MessageId);
        }
    }
}