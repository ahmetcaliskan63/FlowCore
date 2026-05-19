using FlowCore.Core.Common;
using FlowCore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class StatusHistory : BaseEntity
    {
        public WorkflowType EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public LeaveStatus OldStatus { get; set; }
        public LeaveStatus NewStatus { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
