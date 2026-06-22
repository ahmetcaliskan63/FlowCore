using AutoMapper;
using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
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
    }
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IMapper _mapper;   

        public UpdateDepartmentCommandHandler(IRepository<Department> departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository
                .Table.FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);
            if(department == null)
            {
                throw new BusinessException("Güncellenmek istenen aktif bir departman bulunamadı.");
            }
            var duplicateDepartment = await _departmentRepository
                .Table.FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == request.DepartmentName.ToLower() && d.Id != request.Id && !d.IsDeleted, cancellationToken);
            if(duplicateDepartment != null)
            {
                throw new BusinessException($"'{request.DepartmentName}' isimli bir departman sistemde zaten mevcut.");
            }
            department.DepartmentName = request.DepartmentName;
            department.UpdatedAt = DateTime.UtcNow;
            await _departmentRepository.UpdateAsync(department);

            return _mapper.Map<DepartmentDto>(department);
        }
    }
}
