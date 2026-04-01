using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Commons.Core.Data.Context;
using Estudaki.Modules.Comunications.Infrastructure.Extensions;
using Estudaki.Modules.Questions.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Estudaki.Infrastructure.Crosscutting;

public static class ConfigureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.RetryReads = true;
            settings.RetryWrites = true;

            return new MongoClient(settings);
        });
        services.AddScoped<IMongoContext, MongoContext>();
        services.AddCQRS();

        // Modules
        services.AddQuestionsModule();
        services.AddComunicationsInfrastructure();
    }
}
