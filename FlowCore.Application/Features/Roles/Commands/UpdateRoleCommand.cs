using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<RoleDto>
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
    }
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
    {
        private readonly IRepository<Role> _roleRepository;
        public UpdateRoleCommandHandler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (existingRole == null)
            {
                throw new Exception("Düncellenmek istenen rol bulunamadı.");
            }
            var duplicateRole = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Name.ToLower() == request.RoleName.ToLower() && r.Id != request.Id, cancellationToken);
            if (duplicateRole != null)
            {
                throw new Exception($"'{request.RoleName}' adlı bir rol zaten mevcut.");
            }

            existingRole.Name = request.RoleName;
            existingRole.Description = request.RoleDescription;
            existingRole.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(existingRole);
            
            return new RoleDto
            {
                Id = existingRole.Id,
                Name = existingRole.Name,
                Description = existingRole.Description,
            };
        }
    }
}
