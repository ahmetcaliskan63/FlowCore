using AutoMapper;
using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<RoleDto>
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (role == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir rol bulunamadı.");

            var duplicateRole = await _roleRepository.Table
                .AnyAsync(r => r.Name.ToLower() == request.RoleName.ToLower() && r.Id != request.Id && !r.IsDeleted, cancellationToken);

            if (duplicateRole)
                throw new BusinessException($"'{request.RoleName}' adlı bir rol sistemde zaten mevcut.");

            role.Name = request.RoleName;
            role.Description = request.RoleDescription;
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = request.UpdatedByUserId;

            await _roleRepository.UpdateAsync(role);

            return _mapper.Map<RoleDto>(role);
        }
    }
}
