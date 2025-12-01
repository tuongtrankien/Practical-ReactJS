using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using SimpleApp.Application.Common.Behaviors;

namespace SimpleApp.Application;

public static class ConfigureService
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register MediatR for CQRS pattern
        services.AddMediatR(assembly);

        // Register FluentValidation validators from current assembly
        services.AddValidatorsFromAssembly(assembly);

        // Register MediatR pipeline behavior to run validation before handlers
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}