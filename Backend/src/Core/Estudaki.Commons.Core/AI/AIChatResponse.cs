namespace Estudaki.Commons.Core.AI;

/// <summary>
/// Resposta retornada pela IA para um prompt ou conversação.
/// </summary>
/// <param name="Content">Texto da resposta gerada pela IA.</param>
/// <param name="ModelId">Identificador do modelo utilizado para gerar a resposta.</param>
/// <param name="InputTokens">Quantidade de tokens consumidos na requisição, quando disponível.</param>
/// <param name="OutputTokens">Quantidade de tokens gerados na resposta, quando disponível.</param>
public record AIChatResponse(string Content, string? ModelId = null, long? InputTokens = null, long? OutputTokens = null);
