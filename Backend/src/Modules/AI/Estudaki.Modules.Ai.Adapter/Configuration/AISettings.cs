namespace Estudaki.Modules.Ai.Adapter.Configuration;
/// <summary>
/// Configurações necessárias para integração com o provedor de IA.
/// </summary>
public class AISettings
{
    public const string SectionName = "AI";

    /// <summary>
    /// Chave de acesso à API do provedor de IA.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Modelo utilizado para as requisições de chat/agente.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint para uso com provedores compatíveis com a API da OpenAI.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Tempo máximo, em segundos, aguardado por uma resposta do provedor de IA
    /// antes de a requisição ser cancelada por timeout. Padrão: 100 segundos (1m40s).
    /// </summary>
    public int TimeoutInSeconds { get; set; } = 100;
}

