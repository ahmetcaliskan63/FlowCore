using FlowCore.Application.Features.Tasks.Commands;
using FlowCore.Application.Features.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? assignedToUserId,
            [FromQuery] Guid? createdByUserId,
            [FromQuery] string? status)
        {
            var query = new GetAllAppTasksQuery
            {
                AssignedToUserId = assignedToUserId,
                CreatedByUserId = createdByUserId,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetAppTaskByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppTaskCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAppTaskCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateAppTaskStatusCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromQuery] Guid deletedByUserId)
        {
            var command = new DeleteAppTaskCommand { Id = id, DeletedByUserId = deletedByUserId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
