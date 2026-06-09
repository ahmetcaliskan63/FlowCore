using FlowCore.Application.Features.Notifications.Commands;
using FlowCore.Application.Features.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll([FromRoute] Guid userId, [FromQuery] bool? onlyUnread)
        {
            var query = new GetAllNotificationsQuery { UserId = userId, OnlyUnread = onlyUnread };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            var command = new MarkNotificationAsReadCommand { Id = id, UserId = userId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{userId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead([FromRoute] Guid userId)
        {
            var command = new MarkAllNotificationsAsReadCommand { UserId = userId };
            var count = await _mediator.Send(command);
            return Ok(new { MarkedAsRead = count });
        }
    }
}
