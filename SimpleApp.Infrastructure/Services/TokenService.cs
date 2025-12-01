using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;
using SimpleApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace SimpleApp.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _db;

    public TokenService(IConfiguration configuration, UserManager<User> userManager, ApplicationDbContext db)
    {
        _configuration = configuration;
        _userManager = userManager;
        _db = db;
    }

    public async Task<(string token, DateTime expiresUtc)> CreateAccessTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public async Task<(string token, DateTime expiresUtc)> CreateRefreshTokenAsync(User user)
    {
        // generate a secure random token
        var random = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(random);
        var expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshExpireDays"] ?? "7"));

        var refresh = new RefreshToken
        {
            Token = token,
            UserId = user.Id,
            CreatedUtc = DateTime.UtcNow,
            ExpiresUtc = expires,
            Revoked = false
        };

        // save to DB
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync();

        return (token, expires);
    }

    public async Task<(string accessToken, DateTime accessExpiresUtc, string refreshToken, DateTime refreshExpiresUtc)> RefreshAccessTokenAsync(string refreshToken)
    {
        var existing = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (existing == null || existing.Revoked || existing.ExpiresUtc <= DateTime.UtcNow)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        // get the user
        var user = await _userManager.FindByIdAsync(existing.UserId);
        if (user == null) throw new SecurityTokenException("Invalid refresh token user");

        // revoke existing
        existing.Revoked = true;
        _db.RefreshTokens.Update(existing);

        // create new refresh token
        var (newRefreshToken, refreshExp) = await CreateRefreshTokenAsync(user);

        // create new access token
        var (accessToken, accessExp) = await CreateAccessTokenAsync(user);

        await _db.SaveChangesAsync();

        return (accessToken, accessExp, newRefreshToken, refreshExp);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var existing = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (existing == null) return;
        existing.Revoked = true;
        _db.RefreshTokens.Update(existing);
        await _db.SaveChangesAsync();
    }

    public async Task RevokeAllRefreshTokensForUserAsync(string userId)
    {
        var tokens = await _db.RefreshTokens.Where(r => r.UserId == userId && !r.Revoked).ToListAsync();
        if (!tokens.Any()) return;
        foreach (var t in tokens) t.Revoked = true;
        _db.RefreshTokens.UpdateRange(tokens);
        await _db.SaveChangesAsync();
    }
}