using FlowCore.Core.Common;
using FlowCore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class WorkflowStep :BaseEntity
    {
        public Guid WorkflowId { get; set; }
        public virtual Workflow? Workflow { get; set; }
        public int StepOrder { get; set; }
        public Guid? RequiredRoleId { get; set; }
        public virtual Role? RequiredRole { get; set; }
        public ActionType ActionType { get; set; }
    }
}
