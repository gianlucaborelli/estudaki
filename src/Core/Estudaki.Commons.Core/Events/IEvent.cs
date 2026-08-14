namespace Estudaki.Commons.Core.Events;

/// <summary>
/// Marker interface para eventos de domínio.
/// Define um contrato base para todos os eventos que podem ser despachados no sistema.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// ID único do evento
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// Tipo do evento (nome qualificado)
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Timestamp de quando o evento foi criado
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// ID de correlação para rastreability distribuída
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// ID do agregado que desencadeou o evento
    /// </summary>
    string? AggregateId { get; }
}
