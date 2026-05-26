using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Roles.Commands
{
    public class CreateRoleCommand : IRequest<RoleDto>
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IRepository<Role> _roleRepository;
        public CreateRoleCommandHandler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Name.ToLower() == request.RoleName.ToLower() && !r.IsDeleted, cancellationToken);
            if (existingRole != null)
            {
                throw new Exception("Bu rol zaten mevcut.");
            }

            var newRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = request.RoleName,
                Description = request.RoleDescription,
                CreatedAt = DateTime.UtcNow
            };
            await _roleRepository.AddAsync(newRole);
            return new RoleDto
            {
                Id = newRole.Id,
                Name = newRole.Name,
                Description = newRole.Description,
            };
        }
    }

}
