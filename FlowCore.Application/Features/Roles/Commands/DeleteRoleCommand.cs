using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Roles.Commands
{
    public class DeleteRoleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid DeletedByUserId { get; set; }
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
            var role = await _roleRepository.Table
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (role == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir rol bulunamadı.");

            var hasAssignedUsers = await _userRepository.Table
                .AnyAsync(u => u.RoleId == request.Id && !u.IsDeleted, cancellationToken);

            if (hasAssignedUsers)
                throw new BusinessException("Bu role atanmış aktif kullanıcılar bulunmaktadır. Önce kullanıcıların rollerini güncelleyin.");

            role.IsDeleted = true;
            role.DeletedAt = DateTime.UtcNow;
            role.DeletedBy = request.DeletedByUserId;

            await _roleRepository.UpdateAsync(role);

            return true;
        }
    }
}
