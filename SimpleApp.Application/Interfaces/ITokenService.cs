using SimpleApp.Domain.Entities;

namespace SimpleApp.Application.Interfaces
{
    public interface ITokenService
    {
        Task<(string token, DateTime expiresUtc)> CreateAccessTokenAsync(User user);
    }
}