using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionById;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Infrastructure.Data.Context;
using Estudaki.Modules.Questions.Infrastructure.Data.Mappings;
using Estudaki.Modules.Questions.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Modules.Questions.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestionsModule(
        this IServiceCollection services,
        string connectionString,
        string databaseName)
    {
        MongoDbMappings.RegisterMappings();

        services.AddSingleton<IMongoContext>(sp => new MongoContext(connectionString, databaseName));

        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IPublicNoticeRepository, PublicNoticeRepository>();

        services.AddCQRSHandlers(typeof(GetQuestionByIdQueryHandler).Assembly);        
        services.AddCQRSHandlers(typeof(GetFilterParametersQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(SearchQuestionsPaginatedQueryHandler).Assembly);

        return services;
    }
}
