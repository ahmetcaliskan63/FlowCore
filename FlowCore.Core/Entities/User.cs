using FlowCore.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Core.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid? RoleId { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
