
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Application.DTOs.Auth;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;

namespace SimpleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IHostEnvironment _env;

    public AuthController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager, ITokenService tokenService,
        IHostEnvironment env)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        var roleExists = await _roleManager.RoleExistsAsync("User");
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }
        await _userManager.AddToRoleAsync(user, "User");
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized();

        var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signIn.Succeeded) return Unauthorized();

        var (accessToken, accessExp) = await _tokenService.CreateAccessTokenAsync(user);
        var (refreshToken, refreshExp) = await _tokenService.CreateRefreshTokenAsync(user);

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = accessExp.ToUniversalTime()
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = refreshExp.ToUniversalTime()
        };

        Response.Cookies.Append("jwt", accessToken, accessCookieOptions);
        Response.Cookies.Append("refresh", refreshToken, refreshCookieOptions);

        return Ok(new { accessExpiresAtUtc = accessExp, refreshExpiresAtUtc = refreshExp });
    }

    // Development-only helper for Swagger UI: authenticate and set auth cookies so Swagger can call protected endpoints.
    [HttpPost("swagger-login")]
    public async Task<IActionResult> SwaggerLogin(LoginDto dto)
    {
        if (!_env.IsDevelopment()) return NotFound();

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized();

        var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signIn.Succeeded) return Unauthorized();

        var (accessToken, accessExp) = await _tokenService.CreateAccessTokenAsync(user);
        var (refreshToken, refreshExp) = await _tokenService.CreateRefreshTokenAsync(user);

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = accessExp.ToUniversalTime()
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = refreshExp.ToUniversalTime()
        };

        Response.Cookies.Append("jwt", accessToken, accessCookieOptions);
        Response.Cookies.Append("refresh", refreshToken, refreshCookieOptions);

        // Do not return the token in the body even in dev: Swagger will receive Set-Cookie headers.
        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Try to revoke refresh token referenced by cookie
        if (Request.Cookies.TryGetValue("refresh", out var refreshToken))
        {
            await _tokenService.RevokeRefreshTokenAsync(refreshToken);
        }

        // If user is authenticated, revoke all tokens for the user
        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await _tokenService.RevokeAllRefreshTokensForUserAsync(userId);
        }

        // Delete cookies
        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("refresh");
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refresh", out var refreshToken))
            return Unauthorized();

        try
        {
            var (accessToken, accessExp, newRefresh, refreshExp) = await _tokenService.RefreshAccessTokenAsync(refreshToken);

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = accessExp.ToUniversalTime()
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = refreshExp.ToUniversalTime()
            };

            Response.Cookies.Append("jwt", accessToken, accessCookieOptions);
            Response.Cookies.Append("refresh", newRefresh, refreshCookieOptions);

            return Ok(new { accessExpiresAtUtc = accessExp, refreshExpiresAtUtc = refreshExp });
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new { email = user.Email, roles = roles });
    }
}