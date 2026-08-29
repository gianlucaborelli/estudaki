namespace Estudaki.Commons.Core.Events;

/// <summary>
/// Despachador de eventos que gerencia publicação e entrega a handlers.
/// Responsável por descobrir e invocar handlers registrados para cada tipo de evento.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Publica um evento de forma assíncrona.
    /// Todos os handlers registrados para este tipo serão invocados.
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento</typeparam>
    /// <param name="event">Instância do evento</param>
    /// <param name="cancellationToken">Token para cancelamento</param>
    /// <returns>Resultado do processamento</returns>
    Task<IEventHandlingResult> PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Publica um evento de forma síncrona.
    /// Todos os handlers registrados para este tipo serão invocados.
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento</typeparam>
    /// <param name="event">Instância do evento</param>
    /// <returns>Resultado do processamento</returns>
    IEventHandlingResult Publish<TEvent>(TEvent @event)
        where TEvent : IEvent;

    /// <summary>
    /// Publica um evento de forma assíncrona sem aguardar o resultado.
    /// Fire-and-forget: handlers são invocados mas não aguardados.
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento</typeparam>
    /// <param name="event">Instância do evento</param>
    /// <returns>Task iniciada (não aguarda conclusão)</returns>
    Task PublishFireAndForgetAsync<TEvent>(TEvent @event)
        where TEvent : IEvent;
}

/// <summary>
/// Exceção lançada quando ocorre erro durante o despacho de evento
/// </summary>
public class EventDispatchException : Exception
{
    public EventDispatchException(string message) : base(message)
    {
    }

    public EventDispatchException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção lançada quando um evento é inválido
/// </summary>
public class InvalidEventException : Exception
{
    public InvalidEventException(string message) : base(message)
    {
    }

    public InvalidEventException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
