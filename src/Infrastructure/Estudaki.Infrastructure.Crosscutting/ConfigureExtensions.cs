using Amazon;
using Amazon.S3;
using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Storage;
using Estudaki.Infrastructure.Crosscutting.Storage;
using Estudaki.Modules.Comunications.Infrastructure.Extensions;
using Estudaki.Modules.Questions.Infrastructure.Extensions;
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
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = configuration.GetSection(StorageSettings.SectionName).Get<StorageSettings>();
            var regionEndpoint = RegionEndpoint.GetBySystemName(s3Settings?.Region ?? "us-east-1");
            return new AmazonS3Client(s3Settings?.AccessKey, s3Settings?.SecretKey, regionEndpoint);
        });
        services.AddScoped<IStorageService, S3StorageService>();

        // Modules
        services.AddQuestionsModule();
        services.AddComunicationsInfrastructure();
    }
}
