using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Departments.Commands
{
    public class CreateDepartmentCommand : IRequest<DepartmentDto>
    {
        public string DepartmentName { get; set; } = string.Empty;
        public Guid CreatorUserId { get; set;}

    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
    {
        private readonly IRepository<Department> _departmentRepository;

        public CreateDepartmentCommandHandler(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _departmentRepository.Table
                .FirstOrDefaultAsync(d => d.DepartmentName == request.DepartmentName && !d.IsDeleted, cancellationToken);

            if (existingDepartment != null)
            {
                throw new Exception($"'{request.DepartmentName}' isimli bir departman sistemde zaten mevcut.");
            }

            var newDepartment = new Department
            {
                Id = Guid.NewGuid(),
                DepartmentName = request.DepartmentName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatorUserId,
                IsDeleted = false
            };

            await _departmentRepository.AddAsync(newDepartment);

            return new DepartmentDto
            {
                Id = newDepartment.Id,
                DepartmentName = newDepartment.DepartmentName
            };
        }
    }
}