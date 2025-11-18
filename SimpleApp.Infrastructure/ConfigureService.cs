using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;
using SimpleApp.Infrastructure.Data;
using SimpleApp.Infrastructure.Repositories;
using SimpleApp.Infrastructure.Services;

namespace SimpleApp.Infrastructure;

public static class ConfigureService
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredUniqueChars = 0;
            opt.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>()
        .AddSignInManager<SignInManager<User>>()
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}