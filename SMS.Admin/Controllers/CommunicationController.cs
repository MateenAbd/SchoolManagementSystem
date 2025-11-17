using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Commands.Communication;
using SMS.Application.Queries.Communication;

namespace SMS.Admin.Controllers
{
    [Authorize]
    public class CommunicationController : Controller
    {
        private readonly IMediator _mediator;
        public CommunicationController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        private int CurrentUserId => int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        [HttpGet]
        public async Task<IActionResult> Conversations(CancellationToken token)
        {
            var list = await _mediator.Send(new GetConversationsQuery { UserId = CurrentUserId }, token);
            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> Messages(int conversationId, int? beforeMessageId, int pageSize = 50, CancellationToken token = default)
        {
            var msgs = await _mediator.Send(new GetMessagesQuery
            {
                ConversationId = conversationId,
                UserId = CurrentUserId,
                PageSize = pageSize,
                BeforeMessageId = beforeMessageId
            }, token);
            return Json(msgs);
        }

        [HttpPost]
        public async Task<IActionResult> StartDirect([FromBody] int otherUserId, CancellationToken token)
        {
            var id = await _mediator.Send(new StartDirectConversationCommand { UserAId = CurrentUserId, UserBId = otherUserId }, token);
            return Ok(new { success = id > 0, conversationId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageCommand cmd, CancellationToken token)
        {
            cmd.SenderUserId = CurrentUserId;
            var id = await _mediator.Send(cmd, token);
            return Ok(new { success = id > 0, messageId = id });
        }

        [HttpPost]
        public async Task<IActionResult> AckRead([FromBody] AckReadCommand cmd, CancellationToken token)
        {
            cmd.UserId = CurrentUserId;
            var updated = await _mediator.Send(cmd, token);
            return Ok(new { success = updated > 0 });
        }

        [HttpGet]
        public async Task<IActionResult> Presence(int userId, CancellationToken token)
        {
            var p = await _mediator.Send(new GetPresenceQuery { UserId = userId }, token);
            return Json(p);
        }

        [HttpGet]
        public IActionResult Index() => View();
    }
}