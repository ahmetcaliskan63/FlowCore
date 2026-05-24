using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public Guid Id {  get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        public UpdateUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Table
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null) {
                throw new Exception("User not found");
            }
            user.FullName = request.FullName;
            user.Email = request.Email;
            await _userRepository.UpdateAsync(user);
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                DepartmentName = user.Department?.DepartmentName ?? string.Empty,
                TotalLeaveCredits = user.TotalLeaveCredits,
                RemainingLeaveCredits = user.RemainingLeaveCredits
            };

        }
    }
}
