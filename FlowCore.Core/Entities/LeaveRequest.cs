using FlowCore.Core.Common;
using FlowCore.Core.Enums;

namespace FlowCore.Core.Entities
{
    public class LeaveRequest :BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = string.Empty;
        public ProcessStatus Status { get; set; } = ProcessStatus.OnayBekliyor;
    }
}
