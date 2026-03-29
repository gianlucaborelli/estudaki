using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Commons.Core.Data.Context;
using Estudaki.Modules.Questions.Infrastructure.Extensions;
using Estudaki.Modules.Comunications.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Infrastructure.Crosscutting;

public static class ConfigureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMongoContext, MongoContext>();
        services.AddCQRS();

        // Modules
        services.AddQuestionsModule();
        services.AddComunicationsInfrastructure();
    }
}
