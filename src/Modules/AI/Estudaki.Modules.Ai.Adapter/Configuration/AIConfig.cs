using System.ClientModel;
using Estudaki.Commons.Core.AI;
using Estudaki.Modules.Ai.Adapter.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace Estudaki.Modules.Ai.Adapter.Configuration;

public static class AIConfig
{
    public static void AddAIInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AISettings>(configuration.GetSection(AISettings.SectionName));
        services.AddSingleton<IChatClient>(sp =>
        {
            var aiSettings = configuration.GetSection(AISettings.SectionName).Get<AISettings>()
                ?? throw new InvalidOperationException("AI configuration not found in appsettings.json");

            var options = new OpenAIClientOptions();
            if (!string.IsNullOrWhiteSpace(aiSettings.BaseUrl))
                options.Endpoint = new Uri(aiSettings.BaseUrl);

            var openAIClient = new OpenAIClient(new ApiKeyCredential(aiSettings.ApiKey), options);

            return openAIClient.GetChatClient(aiSettings.Model).AsIChatClient();
        });

        services.AddScoped<IAIService, OpenAIChatService>();
    }
}
