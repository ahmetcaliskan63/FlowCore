using FlowCore.Application.Features.Departments.Commands;
using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Application.Features.Departments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var query = new GetAllDepartmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(Guid id)
        {
            var query = new GetDepartmentByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDepartmentById), new { id = result.Id }, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] Guid id)
        {
            var command = new DeleteDepartmentCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
