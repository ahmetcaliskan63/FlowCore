using FlowCore.Core.Entities;

namespace FlowCore.Core.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
    }
}
