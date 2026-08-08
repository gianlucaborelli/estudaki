namespace Estudaki.Commons.Core.AI;

/// <summary>
/// Representa uma mensagem trocada em uma conversação com a IA.
/// </summary>
public record AIChatMessage(AIChatRole Role, string Content)
{
    public static AIChatMessage FromSystem(string content) => new(AIChatRole.System, content);

    public static AIChatMessage FromUser(string content) => new(AIChatRole.User, content);

    public static AIChatMessage FromAssistant(string content) => new(AIChatRole.Assistant, content);
}
