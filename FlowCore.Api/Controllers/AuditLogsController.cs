using FlowCore.Application.Features.AuditLogs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditLogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? userId,
            [FromQuery] string? entityName,
            [FromQuery] Guid? entityId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = new GetAuditLogsQuery
            {
                UserId = userId,
                EntityName = entityName,
                EntityId = entityId,
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
