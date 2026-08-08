using System.ClientModel;
using Estudaki.Commons.Core.AI;
using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Modules.Ai.Adapter.Configuration;
using Estudaki.Modules.Ai.Adapter.Services;
using Estudaki.Modules.Ai.Application.Commands;
using Estudaki.Modules.Ai.Application.Interfaces;
using Estudaki.Modules.Ai.Infrastructure.Repository;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using OpenAI;

namespace Estudaki.Modules.Ai.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IAiRepository, AIRepository>();
        services.AddScoped<IAIService, OpenAIChatService>();

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

        // Remova esta linha:
        services.AddValidatorsFromAssembly(typeof(CreateAIPromptCommandValidator).Assembly);
        
        // E adicione registros manuais:
        //services.AddScoped<IValidator<CreateAIPromptCommand>, CreateAIPromptCommandValidator>();
        // Adicione outros validadores conforme necessário

        services.AddCQRSHandlers(typeof(CreateAIPromptCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(DeleteAIPromptCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UpdateAIPromptCommandHandler).Assembly);

        return services;
    }
}
