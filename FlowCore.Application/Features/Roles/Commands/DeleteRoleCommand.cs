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
    public class DeleteRoleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        public DeleteRoleCommandHandler(IRepository<Role> roleRepository, IRepository<User> userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (existingRole == null)
            {
                throw new Exception("Silinmek istenen rol bulunamadı.");
            }
            var hasAssignedUsers = await _userRepository.Table
                .AnyAsync(u=> u.RoleId == request.Id , cancellationToken);
            if (hasAssignedUsers)
            {
                throw new Exception("Bu rol, atanmış kullanıcılar içeriyor. Önce kullanıcıların rollerini güncelleyin.");
            }
            await _roleRepository.DeleteAsync(existingRole);
            return true;
        }
    }
}
