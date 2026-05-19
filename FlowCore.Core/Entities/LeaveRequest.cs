using FlowCore.Core.Common;
using FlowCore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class LeaveRequest :BaseEntity
    {
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; } = LeaveStatus.OnayBekliyor;
    }
}
