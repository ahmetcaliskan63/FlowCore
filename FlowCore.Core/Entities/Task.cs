using FlowCore.Core.Common;
using FlowCore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class Task : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.orta;
        public DateTime? DueDate { get; set; }
    }
}
