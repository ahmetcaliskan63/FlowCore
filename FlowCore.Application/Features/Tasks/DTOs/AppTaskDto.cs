using FlowCore.Core.Enums;

namespace FlowCore.Application.Features.Tasks.DTOs
{
    public class AppTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public Guid? AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
