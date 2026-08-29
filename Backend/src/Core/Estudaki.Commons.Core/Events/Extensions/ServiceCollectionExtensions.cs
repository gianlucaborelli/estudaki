using System.Reflection;
using Estudaki.Commons.Core.Events.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Commons.Core.Events.Extensions;

/// <summary>
/// Extensões para registrar o sistema de eventos na injeção de dependência.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona o despachador de eventos e registra handlers automaticamente.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>IServiceCollection para method chaining</returns>
    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        return services;
    }

    /// <summary>
    /// Registra todos os handlers de eventos de um ou mais assemblies.
    /// Busca por tipos que implementam IEventHandler&lt;TEvent&gt; ou IEventHandlerSync&lt;TEvent&gt;.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assemblies">Assemblies para escanear. Se vazio, usa o assembly chamador.</param>
    /// <returns>IServiceCollection para method chaining</returns>
    public static IServiceCollection AddEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
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
            .Where(x => IsEventHandlerInterface(x.interfaceType));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.interfaceType, handler.type);
        }
    }

    private static bool IsEventHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericType = type.GetGenericTypeDefinition();

        return genericType == typeof(IEventHandler<>) ||
               genericType == typeof(IEventHandlerSync<>);
    }
}
