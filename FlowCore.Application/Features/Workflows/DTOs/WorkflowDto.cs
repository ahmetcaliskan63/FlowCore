using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Workflows.DTOs
{
    public class WorkflowDto
    {
        public Guid Id { get; set; }

        public string WorkflowName { get; set; } = string.Empty;
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}
