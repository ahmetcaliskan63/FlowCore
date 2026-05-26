using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        public Guid Id { get; set; }
    }
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRepository<Role> _roleRepository;
        public GetRoleByIdQueryHandler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role == null) {
                throw new Exception("Belirtilen ID'ye sahip rol bulunamadı.");
            }
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }
    }
}
