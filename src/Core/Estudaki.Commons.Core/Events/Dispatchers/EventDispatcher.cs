using System.Collections.Concurrent;

namespace Estudaki.Commons.Core.Events.Dispatchers;

/// <summary>
/// Implementação de despachador de eventos.
/// Gerencia handlers registrados e coordena a publicação de eventos.
/// </summary>
public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, List<Type>> _handlersRegistry;
    private static readonly object _registryLock = new();

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _handlersRegistry = new ConcurrentDictionary<Type, List<Type>>();
    }

    public async Task<IEventHandlingResult> PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventType = typeof(TEvent);
        var handlers = GetHandlers(eventType);

        if (!handlers.Any())
        {
            return new EventHandlingResult
            {
                Success = true,
                ErrorMessage = null,
                Exception = null,
                HandlersExecuted = 0
            };
        }

        var tasks = new List<Task>();
        var executedCount = 0;
        Exception? lastException = null;

        foreach (var handlerType in handlers)
        {
            try
            {
                // Tenta resolver como handler assincron
                if (TryResolveAsyncHandler(eventType, handlerType, out var asyncHandler))
                {
                    var task = InvokeAsyncHandlerDynamic(asyncHandler, @event, handlerType, eventType, cancellationToken);
                    tasks.Add(task);
                    executedCount++;
                }
                // Ou tenta como handler síncrono
                else if (TryResolveSyncHandler(eventType, handlerType, out var syncHandler))
                {
                    InvokeSyncHandlerDynamic(syncHandler, @event, handlerType, eventType);
                    executedCount++;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        }

        if (tasks.Any())
        {
            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        }

        return new EventHandlingResult
        {
            Success = lastException == null,
            ErrorMessage = lastException?.Message,
            Exception = lastException,
            HandlersExecuted = executedCount
        };
    }

    public IEventHandlingResult Publish<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventType = typeof(TEvent);
        var handlers = GetHandlers(eventType);

        if (!handlers.Any())
        {
            return new EventHandlingResult
            {
                Success = true,
                ErrorMessage = null,
                Exception = null,
                HandlersExecuted = 0
            };
        }

        var executedCount = 0;
        Exception? lastException = null;

        foreach (var handlerType in handlers)
        {
            try
            {
                // Tenta resolver como handler síncrono
                if (TryResolveSyncHandler(eventType, handlerType, out var syncHandler))
                {
                    InvokeSyncHandlerDynamic(syncHandler, @event, handlerType, eventType);
                    executedCount++;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        }

        return new EventHandlingResult
        {
            Success = lastException == null,
            ErrorMessage = lastException?.Message,
            Exception = lastException,
            HandlersExecuted = executedCount
        };
    }

    public async Task PublishFireAndForgetAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        _ = PublishAsync(@event, CancellationToken.None);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    private List<Type> GetHandlers(Type eventType)
    {
        if (_handlersRegistry.TryGetValue(eventType, out var cached))
            return cached;

        var asyncHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var syncHandlerType = typeof(IEventHandlerSync<>).MakeGenericType(eventType);

        // Busca handlers registrados no DI
        var handlers = new List<Type>();

        try
        {
            // Tenta obter todos os handlers
            var asyncHandlers = GetServicesOfType(_serviceProvider, asyncHandlerType);
            if (asyncHandlers != null)
            {
                handlers.AddRange(asyncHandlers.Select(h => h.GetType()));
            }

            var syncHandlers = GetServicesOfType(_serviceProvider, syncHandlerType);
            if (syncHandlers != null)
            {
                handlers.AddRange(syncHandlers.Select(h => h.GetType()));
            }
        }
        catch
        {
            // Se não conseguir descobrir, retorna lista vazia
        }

        lock (_registryLock)
        {
            _handlersRegistry.TryAdd(eventType, handlers);
        }

        return handlers;
    }

    private bool TryResolveAsyncHandler(Type eventType, Type handlerType, out object? handler)
    {
        handler = null;

        try
        {
            var asyncHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            if (!asyncHandlerType.IsAssignableFrom(handlerType))
                return false;

            handler = _serviceProvider.GetService(handlerType);
            return handler != null;
        }
        catch
        {
            return false;
        }
    }

    private bool TryResolveSyncHandler(Type eventType, Type handlerType, out object? handler)
    {
        handler = null;

        try
        {
            var syncHandlerType = typeof(IEventHandlerSync<>).MakeGenericType(eventType);

            if (!syncHandlerType.IsAssignableFrom(handlerType))
                return false;

            handler = _serviceProvider.GetService(handlerType);
            return handler != null;
        }
        catch
        {
            return false;
        }
    }

    private async Task InvokeAsyncHandlerDynamic(object handler, IEvent @event, Type handlerType, Type eventType, CancellationToken cancellationToken)
    {
        try
        {
            var method = handlerType.GetMethod("HandleAsync")
                ?? throw new InvalidOperationException($"Handler {handlerType.Name} não possui método HandleAsync");

            var result = method.Invoke(handler, new object[] { @event, cancellationToken });

            if (result is Task task)
            {
                await task.ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // Handlers podem falhar sem interromper outros
            // Log pode ser adicionado aqui
        }
    }

    private void InvokeSyncHandlerDynamic(object handler, IEvent @event, Type handlerType, Type eventType)
    {
        try
        {
            var method = handlerType.GetMethod("Handle")
                ?? throw new InvalidOperationException($"Handler {handlerType.Name} não possui método Handle");

            method.Invoke(handler, new object[] { @event });
        }
        catch (Exception ex)
        {
            // Handlers podem falhar sem interromper outros
            // Log pode ser adicionado aqui
        }
    }

    private static IEnumerable<object>? GetServicesOfType(IServiceProvider provider, Type serviceType)
    {
        try
        {
            var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var instance = provider.GetService(type);

            if (instance is System.Collections.IEnumerable enumerable)
            {
                return enumerable.Cast<object>();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Implementação interna do resultado de processamento
    /// </summary>
    private class EventHandlingResult : IEventHandlingResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public Exception? Exception { get; init; }
        public int HandlersExecuted { get; init; }
    }
}
