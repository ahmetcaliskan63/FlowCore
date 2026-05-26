using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Departments.Commands
{
    public class UpdateDepartmentCommand:IRequest<DepartmentDto>
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public Guid UpdatedByUserId { get; set; }
    }
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
    {
        private readonly IRepository<Department> _departmentRepository;

        public UpdateDepartmentCommandHandler(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository
                .Table.FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);
            if(department == null)
            {
                throw new Exception("Güncellenmek istenen aktif bir departman bulunamadı.");
            }
            var duplicateDepartment = await _departmentRepository
                .Table.FirstOrDefaultAsync(d => d.DepartmentName == request.DepartmentName && d.Id != request.Id && !d.IsDeleted, cancellationToken);
            if(duplicateDepartment != null)
            {
                throw new Exception($"'{request.DepartmentName}' isimli bir departman sistemde zaten mevcut.");
            }
            department.DepartmentName = request.DepartmentName;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = request.UpdatedByUserId;
            await _departmentRepository.UpdateAsync(department);

            return new DepartmentDto
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName
            };
        }
    }
}
