using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public GetAllRolesQueryHandler(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.Table
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Name)
                .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return roles;
        }
    }
}
