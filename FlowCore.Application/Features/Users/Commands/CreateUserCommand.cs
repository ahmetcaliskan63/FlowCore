using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
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

        public CreateUserCommandHandler(IRepository<User> userRepository, IRepository<Department> departmentRepository)
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {   
            var existingUser = await _userRepository.Table
                .AnyAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser)
            {
                throw new Exception("Bu e-posta adresi sistemde zaten kayıtlı.");
            }
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);

            string randomPassword = Guid.NewGuid().ToString().Substring(0, 8);
            int defaultLeaveCredits = 14;
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                Password = randomPassword,
                TotalLeaveCredits = defaultLeaveCredits,
                RemainingLeaveCredits = defaultLeaveCredits,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            Console.WriteLine($"[E-Posta Simülasyonu] {user.FullName} için şifre üretildi: {randomPassword}");
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                DepartmentName = department?.DepartmentName ?? "Departman Atanmadı",
                TotalLeaveCredits = user.TotalLeaveCredits,
                RemainingLeaveCredits = user.RemainingLeaveCredits
            };
        }
    }
}