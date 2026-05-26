using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Departments.Queries
{
    public class GetDepartmentByIdQuery : IRequest<DepartmentDto>
    {
        public Guid Id { get; set; }
    }
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
    {
        private readonly IRepository<Department> _departmentRepository;
        public GetDepartmentByIdQueryHandler(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.Table
                .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);
            if (department == null)
            {
                throw new Exception($"'{request.Id}'bu id'ye sahip bir departman bulunamadı.");
            }
            return new DepartmentDto
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName
            };
        }
    }
}
