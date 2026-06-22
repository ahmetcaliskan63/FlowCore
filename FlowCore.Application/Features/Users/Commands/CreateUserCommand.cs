using AutoMapper;
using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(
            IRepository<User> userRepository, 
            IRepository<Department> departmentRepository,
            IRepository<Role> roleRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {   
            var existingUser = await _userRepository.Table
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && !u.IsDeleted, cancellationToken);

            if (existingUser)
            {
                throw new BusinessException("Bu e-posta adresi sistemde zaten kayıtlı.");
            }
            var departmentExists = await _departmentRepository.Table
                .AnyAsync(d=> d.Id == request.DepartmentId && !d.IsDeleted, cancellationToken);
            if (!departmentExists)
            {
                throw new BusinessException("Seçilen departman sistemde bulunamadı veya silinmiş");
            }
            var roleExists = await _roleRepository.Table
                .AnyAsync(r => r.Id == request.RoleId && !r.IsDeleted, cancellationToken);
            if (!roleExists)
            {
                throw new BusinessException("Seçilen role sistemde bulunamadı veya silinmiş");
            }

            string randomPassword = Guid.NewGuid().ToString().Substring(0, 8);
            int defaultLeaveCredits = 14;
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email.ToLower(),
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                Password = _passwordHasher.Hash(randomPassword), // Hash the password securely
                TotalLeaveCredits = defaultLeaveCredits,
                RemainingLeaveCredits = defaultLeaveCredits,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _userRepository.AddAsync(user);
            Console.WriteLine($"[E-Posta Simülasyonu] {user.FullName} için şifre üretildi: {randomPassword}");
            return _mapper.Map<UserDto>(user);
        }
    }
}