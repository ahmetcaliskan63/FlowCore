using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Roles.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
