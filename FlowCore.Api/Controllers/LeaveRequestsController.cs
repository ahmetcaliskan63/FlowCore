using FlowCore.Application.Features.LeaveRequests.Commands;
using FlowCore.Application.Features.LeaveRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllLeaveRequestsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
        {
            var query = new GetLeaveRequestsByUserIdQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result }, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var query = new GetLeaveRequestByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if(result== null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateLeaveRequestCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromQuery] Guid deletedByUserId)
        {
            var command = new DeleteLeaveRequestCommand { Id = id, DeletedByUserId = deletedByUserId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}