using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionById;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Infrastructure.Data.Mappings;
using Estudaki.Modules.Questions.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Modules.Questions.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestionsModule(
        this IServiceCollection services)
    {
        MongoDbMappings.RegisterMappings();        

        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IPublicNoticeRepository, PublicNoticeRepository>();
        services.AddScoped<IQuestionSupportRepository, QuestionSupportRepository>();
        services.AddScoped<IExamProcessingMetadataRepository, ExamProcessingMetadataRepository>();

        services.AddCQRSHandlers(typeof(GetQuestionByIdQueryHandler).Assembly);        
        services.AddCQRSHandlers(typeof(GetFilterParametersQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(SearchQuestionsPaginatedQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetPublicNoticeListQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetPublicNoticeByIdQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetQuestionsByPublicNoticeIdQueryHandler).Assembly);

        return services;
    }
}
