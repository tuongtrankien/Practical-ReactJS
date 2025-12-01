using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleApp.Application;
using SimpleApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // No clock skew tolerance - tokens expire exactly at the specified time
        };
    })
    .AddCookie(o =>
    {
        // Configure cookie policy used by cookie middleware. The JWT cookie we set manually
        // should be HttpOnly, SameSite=None (for cross-site requests) and Secure in production.
        o.Cookie.SameSite = SameSiteMode.None;
        o.Cookie.HttpOnly = true;
        o.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    });

// When using JWT stored in a cookie, also try to read token from cookie if Authorization header is not provided.
builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var originalEvents = options.Events ?? new JwtBearerEvents();
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                if (context.Request.Cookies.TryGetValue("jwt", out var cookieToken))
                {
                    context.Token = cookieToken;
                }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // If token validation fails (e.g., expired), clear the cookies
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Cookies.Delete("jwt");
                context.Response.Cookies.Delete("refresh");
            }
            return originalEvents.OnAuthenticationFailed != null 
                ? originalEvents.OnAuthenticationFailed(context) 
                : Task.CompletedTask;
        },
        OnChallenge = originalEvents.OnChallenge,
        OnTokenValidated = context =>
        {
            // Additional validation: ensure token is not expired
            var exp = context.Principal?.FindFirst(c => c.Type == "exp")?.Value;
            if (exp != null && long.TryParse(exp, out var expSeconds))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                if (expirationTime <= DateTime.UtcNow)
                {
                    context.Fail("Token has expired");
                    context.Response.Cookies.Delete("jwt");
                    context.Response.Cookies.Delete("refresh");
                }
            }
            return originalEvents.OnTokenValidated != null 
                ? originalEvents.OnTokenValidated(context) 
                : Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("react", p => p
        .WithOrigins("http://localhost:5173", "https://your-react.app")
        .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token."
    });

    // Also allow Swagger to accept a cookie value for `jwt` (useful for manual testing).
    c.AddSecurityDefinition("CookieAuth", new OpenApiSecurityScheme
    {
        Name = "jwt",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Description = "Cookie-based auth. Enter the raw JWT token to send as cookie 'jwt'."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Require cookie auth as well so Swagger UI will show it as an option
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CookieAuth"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("react");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();