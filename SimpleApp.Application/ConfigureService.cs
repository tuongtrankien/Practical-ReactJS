using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleApp.Application;

public static class ConfigureService
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR for CQRS pattern
        services.AddMediatR(typeof(ConfigureService).Assembly);

        return services;
    }
}