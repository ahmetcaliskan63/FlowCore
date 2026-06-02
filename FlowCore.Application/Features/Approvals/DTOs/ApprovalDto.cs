using FlowCore.Core.Enums;
using System;

namespace FlowCore.Application.Features.Approvals.DTOs
{
    public class ApprovalDto
    {
        public Guid Id { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public Guid? RequestId { get; set; }
        public Guid? ApproverUserId { get; set; }
        public string ApproverFullName { get; set; } = string.Empty;
        public string ApproverRole { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
    }
}
