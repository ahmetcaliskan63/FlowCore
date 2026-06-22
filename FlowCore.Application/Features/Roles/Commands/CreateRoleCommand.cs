using AutoMapper;
using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
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
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Name.ToLower() == request.RoleName.ToLower() && !r.IsDeleted, cancellationToken);

            if (existingRole != null)
                throw new BusinessException($"'{request.RoleName}' isimli bir rol sistemde zaten mevcut.");

            var newRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = request.RoleName,
                Description = request.RoleDescription,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _roleRepository.AddAsync(newRole);

            return _mapper.Map<RoleDto>(newRole);
        }
    }
}
