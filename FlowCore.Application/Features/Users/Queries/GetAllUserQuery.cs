using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Users.Queries
{
    public class GetAllUserQuery : MediatR.IRequest<List<UserDto>>
    {
    }

    public class GetAllUserQueryHandler : MediatR.IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IRepository<User> _userRepository;
        public GetAllUserQueryHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Table
                .Include(u => u.Department)
                .ToListAsync(cancellationToken);
            if(user == null)
            {
                throw new Exception("Kullanıcılar bulunamadı.");
            }
            var UserRequests = await _userRepository.GetAllAsync();
            return UserRequests.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                DepartmentName = u.Department?.DepartmentName ?? "Departman Atanmamış",
                TotalLeaveCredits = u.TotalLeaveCredits,
                RemainingLeaveCredits = u.RemainingLeaveCredits
            }).ToList();
        }
    }
}
