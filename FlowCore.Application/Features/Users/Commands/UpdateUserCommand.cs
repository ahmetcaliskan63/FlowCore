using AutoMapper;
using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid? DepartmentId { get; set; }
        public Guid? RoleId { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IRepository<User> userRepository, IRepository<Department> departmentRepository, IRepository<Role> roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Table
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir kullanıcı bulunamadı.");

            var emailConflict = await _userRepository.Table
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != request.Id && !u.IsDeleted, cancellationToken);

            if (emailConflict)
                throw new BusinessException($"'{request.Email}' e-posta adresi başka bir kullanıcı tarafından kullanılmaktadır.");

            if (request.DepartmentId.HasValue)
            {
                var departmentExists = await _departmentRepository.Table
                    .AnyAsync(d => d.Id == request.DepartmentId.Value && !d.IsDeleted, cancellationToken);

                if (!departmentExists)
                    throw new BusinessException("Belirtilen departman bulunamadı veya silinmiş.");

                user.DepartmentId = request.DepartmentId.Value;
            }
            if (request.RoleId.HasValue)
            {
                var roleExists = await _roleRepository.Table
                    .AnyAsync(r => r.Id == request.RoleId.Value && !r.IsDeleted, cancellationToken);
                if (!roleExists)
                    throw new BusinessException("Belirtilen rol bulunamadı veya silinmiş.");
                user.RoleId = request.RoleId.Value;
            }

            user.FullName = request.FullName;
            user.Email = request.Email.ToLower();

            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDto>(user);
        }
    }
}
