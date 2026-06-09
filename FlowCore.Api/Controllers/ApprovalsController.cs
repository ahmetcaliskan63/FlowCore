using FlowCore.Application.Features.Approvals.Commands;
using FlowCore.Application.Features.Approvals.DTOs;
using FlowCore.Application.Features.Approvals.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApprovalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprovalsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetApprovalsByRequestId([FromRoute] Guid requestId)
        {
            var query = new GetApprovalsByRequestIdQuery { RequestId = requestId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<LeaveApprovalResultDto>> CreateLeaveApproval([FromBody] ApproveLeaveRequestCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
