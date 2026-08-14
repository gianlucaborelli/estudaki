namespace Estudaki.Commons.Core.Storage;

/// <summary>
/// Serviço genérico para armazenar e recuperar artefatos JSON no bucket, organizados
/// por um identificador de requisição (lote) e um identificador de item dentro do lote.
/// A chave final gerada segue o padrão: "{container}/{requestId}/{itemId}.json".
/// Pode ser reutilizado por qualquer parte do sistema que precise arquivar respostas
/// estruturadas (ex.: respostas brutas de IA, exportações, snapshots, etc.).
/// </summary>
public interface IJsonArtifactStorage
{
    /// <summary>
    /// Serializa o objeto informado como JSON e faz upload para o bucket, retornando a chave gerada.
    /// </summary>
    /// <typeparam name="TValue">Tipo do objeto a ser serializado.</typeparam>
    /// <param name="container">Nome lógico do container/pasta raiz (ex.: "ai-responses").</param>
    /// <param name="requestId">Identificador único da requisição/lote que originou o artefato.</param>
    /// <param name="itemId">Identificador único do item dentro do lote.</param>
    /// <param name="value">Objeto a ser serializado e armazenado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Chave (path) do arquivo armazenado no bucket.</returns>
    Task<string> SaveAsync<TValue>(
        string container,
        string requestId,
        string itemId,
        TValue value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Constrói a chave (path) do artefato sem realizar upload, útil para persistir referências
    /// antes ou depois da gravação efetiva.
    /// </summary>
    string BuildKey(string container, string requestId, string itemId);
}
