using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<List<RoleDto>>
    {
    }
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IRepository<Role> _roleRepository;
        public GetAllRolesQueryHandler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.Table
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync(cancellationToken);
            return roles;
        }
    }
}
