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
    public class ChangePasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(
            IRepository<User> userRepository, 
            IPasswordHasher passwordHasher,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId.ToString() != _currentUserService.UserId)
            {
                throw new UnauthorizedException("Sadece kendi şifrenizi değiştirebilirsiniz.");
            }

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            if (!_passwordHasher.Verify(request.OldPassword, user.Password))
            {
                throw new BusinessException("Mevcut şifreniz yanlış.");
            }

            user.Password = _passwordHasher.Hash(request.NewPassword);
            
            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}
