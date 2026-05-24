using FlowCore.Application.Features.Approvals.Commands;
using FlowCore.Application.Features.Approvals.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprovalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateLeaveApproval")]
        public async Task<ActionResult<LeaveApprovalResultDto>> CreateLeaveApproval([FromBody] ApproveLeaveRequestCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
