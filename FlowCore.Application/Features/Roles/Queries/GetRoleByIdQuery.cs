using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (role == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir rol bulunamadı.");

            return _mapper.Map<RoleDto>(role);
        }
    }
}
