using SimpleApp.Domain.Entities;

namespace SimpleApp.Application.Interfaces
{
    public interface ITokenService
    {
        Task<(string token, DateTime expiresUtc)> CreateAccessTokenAsync(User user);
        Task<(string token, DateTime expiresUtc)> CreateRefreshTokenAsync(User user);

        /// <summary>
        /// Validate the provided refresh token, rotate it (revoke old, create new) and return a new access token and refresh token with expirations.
        /// </summary>
        Task<(string accessToken, DateTime accessExpiresUtc, string refreshToken, DateTime refreshExpiresUtc)> RefreshAccessTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllRefreshTokensForUserAsync(string userId);
    }
}