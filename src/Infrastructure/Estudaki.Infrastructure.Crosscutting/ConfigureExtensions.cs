using Amazon.Runtime;
using Amazon.S3;
using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Ai.Infrastructure;
using Estudaki.Infrastructure.Crosscutting.Storage;
using Estudaki.Modules.Comunications.Infrastructure.Extensions;
using Estudaki.Modules.Questions.Infrastructure.Extensions;
using Estudaki.Modules.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Estudaki.Infrastructure.Crosscutting;

public static class ConfigureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.RetryReads = true;
            settings.RetryWrites = true;

            return new MongoClient(settings);
        });
        services.AddScoped<IMongoContext, MongoContext>();
        services.AddCQRS();

        // Storage S3
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<StorageSettings>>().Value);

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var storageSettings = configuration.GetSection("AwsS3").Get<StorageSettings>();
            if (storageSettings == null)
                throw new InvalidOperationException("AWS S3 configuration not found in appsettings.json");

            var credentials = new BasicAWSCredentials(storageSettings.AccessKey, storageSettings.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = storageSettings.Region,
                ForcePathStyle = true,
                UseHttp = false,
                Timeout = TimeSpan.FromMinutes(10)
            };

            return new AmazonS3Client(credentials, config);
        });
        services.AddScoped<IStorageService, S3StorageService>();
                
        // Modules
        services.AddQuestionsModule(configuration);
        services.AddComunicationsInfrastructure();
        services.AddIdentityModule(configuration);  
        services.AddAiModule(configuration);
    }
}
