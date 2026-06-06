using FlowCore.Application.Features.WorkflowSteps.Commands;
using FlowCore.Application.Features.WorkflowSteps.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowStepsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkflowStepsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? workflowId)
        {
            var query = new GetAllWorkflowStepsQuery { WorkflowId = workflowId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var query = new GetWorkflowStepByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkflowStepCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWorkflowStepCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromQuery] Guid deletedByUserId)
        {
            var command = new DeleteWorkflowStepCommand { Id = id, DeletedByUserId = deletedByUserId };
            var result = await _mediator.Send(command);
            return NoContent();
        }
    }
}
