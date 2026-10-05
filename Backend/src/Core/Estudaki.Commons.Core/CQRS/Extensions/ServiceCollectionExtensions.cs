using System.Reflection;
using Estudaki.Commons.Core.CQRS.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Commons.Core.CQRS.Extensions;

/// <summary>
/// Extension methods for registering CQRS components in dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds command and query dispatchers to the service collection.
    /// </summary>
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        
        return services;
    }

    /// <summary>
    /// Registers all command and query handlers from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    public static IServiceCollection AddCQRSHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        foreach (var assembly in assemblies)
        {
            RegisterHandlers(services, assembly);
        }

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (type, interfaceType) => new { type, interfaceType })
            .Where(x => IsHandlerInterface(x.interfaceType));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.interfaceType, handler.type);
        }
    }

    private static bool IsHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericType = type.GetGenericTypeDefinition();

        return genericType == typeof(ICommandHandler<>) ||
               genericType == typeof(ICommandHandler<,>) ||
               genericType == typeof(IQueryHandler<,>);
    }
}
