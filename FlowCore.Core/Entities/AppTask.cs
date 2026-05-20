using FlowCore.Core.Common;
using FlowCore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class AppTask : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public virtual User? AssignedToUser { get; set; }
        public AppTaskStatus Status { get; set; } = AppTaskStatus.Yapilacak;
        public TaskPriority Priority { get; set; } = TaskPriority.Orta;
        public DateTime? DueDate { get; set; }
    }
}
