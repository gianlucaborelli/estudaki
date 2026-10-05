namespace Estudaki.Commons.Core.AI;

/// <summary>
/// Serviço responsável por integrar a aplicação com provedores de IA,
/// permitindo interações simples (prompt único), em formato de conversação
/// e no modo agêntico, recebendo dados de entrada e retornando respostas estruturadas.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Envia um único prompt para a IA e retorna a resposta em texto livre.
    /// </summary>
    Task<AIChatResponse> AskAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém o conteúdo de um prompt previamente armazenado no repositório de prompts.
    /// </summary>
    /// <param name="promptName">Nome do prompt a ser obtido.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Conteúdo do prompt.</returns>
    Task<string> GetPromptAsync(string promptName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia um histórico de mensagens para a IA, permitindo manter contexto de conversação.
    /// </summary>
    Task<AIChatResponse> ChatAsync(IEnumerable<AIChatMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executa a IA em modo agêntico: recebe instruções e um conjunto opcional de mensagens/dados de contexto
    /// e retorna a resposta já deserializada e estruturada no tipo <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">Tipo esperado para a resposta estruturada.</typeparam>
    /// <param name="instructions">Instruções (prompt de sistema) que orientam o comportamento do agente.</param>
    /// <param name="messages">Mensagens/dados adicionais de entrada para o agente processar.</param>
    Task<TResponse> RunAgentAsync<TResponse>(
        string instructions,
        IEnumerable<AIChatMessage>? messages = null,
        CancellationToken cancellationToken = default)
        where TResponse : class;
}
