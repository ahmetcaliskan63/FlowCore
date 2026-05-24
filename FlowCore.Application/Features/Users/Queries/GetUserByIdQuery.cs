using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        public GetUserByIdQueryHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userEntity = await _userRepository.Table
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (userEntity == null) { throw new KeyNotFoundException("Kullanıcı bulunamadı."); }

            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                DepartmentName = user.Department?.DepartmentName ?? "Departman yok",
                TotalLeaveCredits = user.TotalLeaveCredits,
                RemainingLeaveCredits = user.RemainingLeaveCredits
            };

        }
    }
}
