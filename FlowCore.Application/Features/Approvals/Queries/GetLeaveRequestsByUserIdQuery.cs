using FlowCore.Application.Features.LeaveRequests.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Approvals.Queries
{
    public class GetLeaveRequestsByUserIdQuery : IRequest<List<LeaveRequestDto>>
    {

    }
}
