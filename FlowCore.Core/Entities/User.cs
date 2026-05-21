using FlowCore.Core.Common;

namespace FlowCore.Core.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid? RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public Guid? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public int TotalLeaveCredits { get; set; }
        public int RemainingLeaveCredits { get; set; }
    }
}
