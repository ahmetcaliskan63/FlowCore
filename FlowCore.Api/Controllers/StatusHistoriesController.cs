using FlowCore.Application.Features.StatusHistories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusHistoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatusHistoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? entityId,
            [FromQuery] string? entityType)
        {
            var query = new GetStatusHistoriesQuery
            {
                EntityId = entityId,
                EntityType = entityType
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
