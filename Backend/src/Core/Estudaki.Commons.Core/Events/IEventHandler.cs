namespace Estudaki.Commons.Core.Events;

/// <summary>
/// Interface base para handlers de eventos.
/// Implementadores podem processar eventos de forma síncrona ou assíncrona.
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Obtém o tipo de evento que este handler processa
    /// </summary>
    Type GetEventType();
}

/// <summary>
/// Handler genérico para processar eventos de um tipo específico.
/// </summary>
/// <typeparam name="TEvent">Tipo do evento a ser processado</typeparam>
public interface IEventHandler<in TEvent> : IEventHandler
    where TEvent : IEvent
{
    /// <summary>
    /// Processa um evento.
    /// </summary>
    /// <param name="event">O evento a ser processado</param>
    /// <param name="cancellationToken">Token para cancelamento</param>
    /// <returns>Task completada quando o processamento termina</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler síncrono para eventos.
/// Alternativa quando processamento assincron não é necessário.
/// </summary>
/// <typeparam name="TEvent">Tipo do evento a ser processado</typeparam>
public interface IEventHandlerSync<in TEvent> : IEventHandler
    where TEvent : IEvent
{
    /// <summary>
    /// Processa um evento sincronamente.
    /// </summary>
    /// <param name="event">O evento a ser processado</param>
    void Handle(TEvent @event);
}

/// <summary>
/// Resultado do processamento de um evento
/// </summary>
public interface IEventHandlingResult
{
    /// <summary>
    /// Indica se o processamento foi bem-sucedido
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Mensagem de erro, se houver
    /// </summary>
    string? ErrorMessage { get; }

    /// <summary>
    /// Exceção que ocorreu durante o processamento
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// Handlers que foram executados
    /// </summary>
    int HandlersExecuted { get; }
}
