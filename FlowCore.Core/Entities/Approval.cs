using FlowCore.Core.Common;
using FlowCore.Core.Enums;

namespace FlowCore.Core.Entities
{
    public class Approval : BaseEntity
    {
        public WorkflowType RequestType { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? ApproverByUserId { get; set; }
        public virtual User? ApproverByUser { get; set; }
        public ProcessStatus Status { get; set; } = ProcessStatus.OnayBekliyor;
        public string Comment { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
    }
}
