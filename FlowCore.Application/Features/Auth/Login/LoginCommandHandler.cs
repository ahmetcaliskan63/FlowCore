using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IRepository<User> userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Table
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            if (user == null)
            {
                throw new BusinessException("Invalid email or password.");
            }

            if (!_passwordHasher.Verify(request.Password, user.Password))
            {
                throw new BusinessException("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException("User account is disabled.");
            }

            var tokenResult = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = tokenResult.Token,
                ExpiresAt = tokenResult.ExpiresAt,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role?.Name
            };
        }
    }
}
