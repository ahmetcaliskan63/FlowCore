using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkflowsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkflows()
        {
            var result = await _mediator.Send(new Application.Features.Workflows.Queries.GetAllWorkflowQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflowById(Guid id)
        {
            var result = await _mediator.Send(new Application.Features.Workflows.Queries.GetWorkflowByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflow([FromBody] Application.Features.Workflows.Commands.CreateWorkflowCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetWorkflowById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkflow(Guid id, [FromBody] Application.Features.Workflows.Commands.UpdateWorkflowCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflow([FromRoute] Guid id, [FromQuery] Guid deletedByUserId)
        {
            var result = await _mediator.Send(new Application.Features.Workflows.Commands.DeleteWorkflowCommand { Id = id, DeletedByUserId = deletedByUserId });
            return NoContent();
        }
    }
}