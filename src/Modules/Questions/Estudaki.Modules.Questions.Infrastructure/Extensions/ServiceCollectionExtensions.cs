using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.Queries.GetAreasPaginated;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Application.Queries.GetImageListByPublicNoticeId;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionById;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Infrastructure.Data;
using Estudaki.Modules.Questions.Infrastructure.Data.Mappings;
using Estudaki.Modules.Questions.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Modules.Questions.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestionsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        MongoDbMappings.RegisterMappings();        

        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IPublicNoticeRepository, PublicNoticeRepository>();
        services.AddScoped<IQuestionSupportRepository, QuestionSupportRepository>();

        var postgresConnectionString = configuration.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");
        services.AddDbContextFactory<QuestionsDbContext>(options =>
            options.UseNpgsql(postgresConnectionString));
        services.AddScoped<IAreaRepository, AreaRepository>();

        services.AddValidatorsFromAssembly(typeof(UploadPublicNoticeFilesCommandValidator).Assembly);

        services.AddCQRSHandlers(typeof(GetQuestionByIdQueryHandler).Assembly);        
        services.AddCQRSHandlers(typeof(GetFilterParametersQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(SearchQuestionsPaginatedQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetPublicNoticeListQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetPublicNoticeByIdQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetQuestionsByExamIdQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetQuestionSupportsByPublicNoticeIdQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetImageListByPublicNoticeIdQueryHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetQuestionsByPublicNoticeIdQueryHandler).Assembly);

        services.AddCQRSHandlers(typeof(AddNewQuestionIntoExamCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(AddExistingQuestionIntoExamCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UploadExamFilesCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UploadQuestionImagesCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UnifyPublicNoticeCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UnifyQuestionCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UpdateQuestionCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UpdatePublicNoticeCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UpdateQuestionSupportCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UpdateExamCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(CreatePublicNoticeCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(CreateQuestionSupportCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(DeleteQuestionCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(DeleteQuestionSupportCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(CreateAreaCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(UpdateAreaCommandHandler).Assembly);
        services.AddCQRSHandlers(typeof(GetAreasPaginatedQueryHandler).Assembly);

        return services;
    }
}
