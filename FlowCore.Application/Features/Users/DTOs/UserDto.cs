using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalLeaveCredits { get; set; }
        public int RemainingLeaveCredits { get; set; }

    }
}
