using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.LeaveRequests.DTOs
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
