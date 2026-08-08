using Estudaki.Commons.Core.AI;
using Estudaki.Modules.Ai.Application.Interfaces;
using Microsoft.Extensions.AI;

namespace Estudaki.Modules.Ai.Adapter.Services;

/// <summary>
/// Implementação de <see cref="IAIService"/> baseada na abstração <see cref="IChatClient"/>
/// do Microsoft.Extensions.AI, compatível com OpenAI e demais provedores compatíveis.
/// </summary>
public class OpenAIChatService : IAIService
{
    private readonly IChatClient _chatClient;
    private readonly IAiRepository _promptRepository;

    public OpenAIChatService(IChatClient chatClient, IAiRepository promptRepository)
    {
        _chatClient = chatClient;
        _promptRepository = promptRepository;
    }

    public async Task<AIChatResponse> AskAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: cancellationToken);

        return MapToChatResponse(response);
    }

    public async Task<AIChatResponse> ChatAsync(IEnumerable<AIChatMessage> messages, CancellationToken cancellationToken = default)
    {
        var chatMessages = messages.Select(MapToChatMessage).ToList();

        var response = await _chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);

        return MapToChatResponse(response);
    }

    public async Task<TResponse> RunAgentAsync<TResponse>(
        string instructions,
        IEnumerable<AIChatMessage>? messages = null,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            var chatMessages = new List<ChatMessage> { new(ChatRole.System, instructions) };

            if (messages is not null)
                chatMessages.AddRange(messages.Select(MapToChatMessage));

            var response = await _chatClient.GetResponseAsync<TResponse>(
                chatMessages,
                useJsonSchemaResponseFormat: false,
                cancellationToken: cancellationToken);

            return response.Result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao executar o agente de IA.", ex);
        }
    }

    private static AIChatResponse MapToChatResponse(ChatResponse response) =>
        new(response.Text, response.ModelId, response.Usage?.InputTokenCount, response.Usage?.OutputTokenCount);

    private static ChatMessage MapToChatMessage(AIChatMessage message) =>
        new(MapToChatRole(message.Role), message.Content);

    private static ChatRole MapToChatRole(AIChatRole role) => role switch
    {
        AIChatRole.System => ChatRole.System,
        AIChatRole.Assistant => ChatRole.Assistant,
        AIChatRole.User => ChatRole.User,
        _ => ChatRole.User
    };

    public async Task<string> GetPromptAsync(string promptName, CancellationToken cancellationToken = default)
    {
        var prompt = await _promptRepository.GetByNameAsync(promptName);
        if (prompt == null) 
            throw new InvalidOperationException($"Prompt '{promptName}' not found.");
        return prompt.Content;
    }
}
