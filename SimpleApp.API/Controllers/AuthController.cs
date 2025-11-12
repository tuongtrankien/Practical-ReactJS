
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Application.DTOs.Auth;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;

namespace SimpleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager; 
        _tokenService = tokenService;
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

        var (token, exp) = await _tokenService.CreateAccessTokenAsync(user);
        return Ok(new { accessToken = token, expiresAtUtc = exp });
    }
}