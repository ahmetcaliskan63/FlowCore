using AutoMapper;
using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
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
        private readonly IMapper _mapper;

        public CreateDepartmentCommandHandler(IRepository<Department> departmentRepository ,IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _departmentRepository.Table
                .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == request.DepartmentName.ToLower() && !d.IsDeleted, cancellationToken);

            if (existingDepartment != null)
            {
                throw new BusinessException($"'{request.DepartmentName}' isimli bir departman sistemde zaten mevcut.");
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

            return _mapper.Map<DepartmentDto>(newDepartment);
        }
    }
}