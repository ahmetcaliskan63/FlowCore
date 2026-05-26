using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Departments.Queries
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>
    {
    }
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IRepository<Department> _departmentRepository;
        public GetAllDepartmentsQueryHandler(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _departmentRepository.Table
                .Where(u => !u.IsDeleted)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    DepartmentName = d.DepartmentName,
                })
                .ToListAsync(cancellationToken);
            return departments;
        }
    }

}