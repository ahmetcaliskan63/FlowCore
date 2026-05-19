using FlowCore.Core.Common;
using FlowCore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class Approval : BaseEntity
    {
        public WorkflowType RequestType { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? ApproverByUserId { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.OnayBekliyor;
        public string Comment { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
    }
}
