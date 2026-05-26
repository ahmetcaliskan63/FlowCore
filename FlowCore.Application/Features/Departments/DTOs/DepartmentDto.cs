using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Departments.DTOs
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

    }
}
