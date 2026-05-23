using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Approvals.DTOs
{
    public class LeaveApprovalResultDto
    {
        public Guid ApprovalId { get; set; }
        public Guid LeaveRequestId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public int RemainingLeaveCredits { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ActionAt { get; set; }
    }
}
