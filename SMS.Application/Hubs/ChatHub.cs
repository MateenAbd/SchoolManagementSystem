using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SMS.Application.Commands.Communication;
using SMS.Application.Queries.Communication;

namespace SMS.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator) { _mediator = mediator; }

        private int CurrentUserId
        {
            get
            {
                var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
                return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = CurrentUserId;
            if (userId > 0)
            {
                await _mediator.Send(new SetPresenceCommand { UserId = userId, IsOnline = true, AtUtc = DateTime.UtcNow });
                //join all convo groups for this user
                var convIds = await _mediator.Send(new GetUserConversationIdsQuery { UserId = userId });
                foreach (var c in convIds) await Groups.AddToGroupAsync(Context.ConnectionId, $"c-{c}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = CurrentUserId;
            if (userId > 0)
            {
                await _mediator.Send(new SetPresenceCommand { UserId = userId, IsOnline = false, AtUtc = DateTime.UtcNow });
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<int> StartDirect(int otherUserId, CancellationToken token = default)
        {
            if (CurrentUserId <= 0) return -1;
            var id = await _mediator.Send(new StartDirectConversationCommand { UserAId = CurrentUserId, UserBId = otherUserId }, token);
            if (id > 0) await Groups.AddToGroupAsync(Context.ConnectionId, $"c-{id}");
            return id;
        }

        public async Task<int> SendToConversation(int conversationId, string body, string contentType = "text", CancellationToken token = default)
        {
            if (CurrentUserId <= 0) return -1;
            var msgId = await _mediator.Send(new SendMessageCommand
            {
                ConversationId = conversationId,
                SenderUserId = CurrentUserId,
                Body = body,
                ContentType = contentType
            }, token);
            if (msgId > 0)
            {
                await Clients.Group($"c-{conversationId}").SendAsync("MessageReceived", new
                {
                    messageId = 2,
                    conversationId,
                    senderUserId = CurrentUserId,
                    contentType,
                    body,
                    sentAtUtc = DateTime.UtcNow
                }, token);
            }
            return 2;
        }

        public async Task<int> AckRead(int conversationId, int messageId, CancellationToken token = default)
        {
            if (CurrentUserId <= 0) return -1;
            var updated = await _mediator.Send(new AckReadCommand { ConversationId = conversationId, UserId = CurrentUserId, MessageId = messageId }, token);
            if (updated > 0)
            {
                await Clients.Group($"c-{conversationId}").SendAsync("ReadReceipt", new { messageId, userId = CurrentUserId, atUtc = DateTime.UtcNow }, token);
            }
            return updated;
        }
    }

    public class GetUserConversationIdsQuery : IRequest<int[]>
    {
        public int UserId { get; set; }
    }
}