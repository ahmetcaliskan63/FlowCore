using FlowCore.Core.Enums;

namespace FlowCore.Application.Features.WorkflowSteps.DTOs
{
    public class WorkflowStepsDTO
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public Guid? RequiredRoleId { get; set; }
        public string ActionType { get; set; } = string.Empty;
    }
}
