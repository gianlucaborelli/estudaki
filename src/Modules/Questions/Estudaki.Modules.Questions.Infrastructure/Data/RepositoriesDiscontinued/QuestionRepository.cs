using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class QuestionRepositoryDiscontinued //: BaseRepository<Question>, IQuestionRepository
{
    //public QuestionRepositoryDiscontinued(IMongoContext context) : base(context)
    //{
    //}

    //public async Task<List<Question>> GetByPublicNoticeId(string publicNoticeId)
    //{
    //    var filter = Builders<Question>.Filter.Eq(q => q.PublicNoticeId, publicNoticeId);
    //    return await DbSet.Find(filter).ToListAsync();
    //}

    //public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
    //{
    //    var builder = Builders<Question>.Filter;
    //    var filters = new List<FilterDefinition<Question>>();

    //    // Filtrar apenas questões publicadas
    //    filters.Add(builder.Eq(q => q.IsPublished, true));

    //    if (filterParameters.TypeQuestions is { Length: > 0 })
    //        filters.Add(builder.In(q => q.Type, filterParameters.TypeQuestions));

    //    if (filterParameters.ExamCategories is { Length: > 0 })
    //    {
    //        // Filtrar por PublicNoticeId que tenham a categoria desejada E que estejam publicados
    //        var publicNoticeBuilder = Builders<PublicNotice>.Filter;
    //        var categoryFilter = publicNoticeBuilder.And(
    //            publicNoticeBuilder.In(pn => pn.ExamCategory, filterParameters.ExamCategories),
    //            publicNoticeBuilder.Eq(pn => pn.IsPublished, true)
    //        );

    //        var publicNoticesCollection = Context.GetCollection<PublicNotice>();
    //        var publicNoticeIdsForFilter = await publicNoticesCollection
    //            .Find(categoryFilter)
    //            .Project(pn => pn.Id)
    //            .ToListAsync();

    //        if (publicNoticeIdsForFilter.Any())
    //            filters.Add(builder.In(q => q.PublicNoticeId, publicNoticeIdsForFilter));
    //    }

    //    if (filterParameters.MainAreas is { Length: > 0 })
    //        filters.Add(builder.In(q => q.MainArea, filterParameters.MainAreas));

    //    if (filterParameters.SubAreas is { Length: > 0 })
    //        filters.Add(builder.In("SubAreas", filterParameters.SubAreas));

    //    var combinedFilter = filters.Any() ? builder.And(filters) : builder.Empty;

    //    // Buscar valores distintos das questões publicadas
    //    var typeQuestionsTask = DbSet.DistinctAsync<string>("Type", combinedFilter);
    //    var mainAreasTask = DbSet.DistinctAsync<string>("MainArea", combinedFilter);
    //    var subAreasTask = DbSet.DistinctAsync<string>("SubAreas", combinedFilter);

    //    // Buscar PublicNoticeIds distintos das questões filtradas
    //    var publicNoticeIdsTask = DbSet.DistinctAsync<string>("PublicNoticeId", combinedFilter);

    //    await Task.WhenAll(typeQuestionsTask, mainAreasTask, subAreasTask, publicNoticeIdsTask);

    //    var typeQuestions = await typeQuestionsTask;
    //    var mainAreas = await mainAreasTask;
    //    var subAreas = await subAreasTask;
    //    var publicNoticeIds = await publicNoticeIdsTask;

    //    var typeQuestionsList = await typeQuestions.ToListAsync();
    //    var mainAreasList = await mainAreas.ToListAsync();
    //    var subAreasList = await subAreas.ToListAsync();
    //    var publicNoticeIdsList = await publicNoticeIds.ToListAsync();

    //    // Buscar ExamCategories distintas dos PublicNotices relacionados às questões (apenas publicados)
    //    var examCategoriesList = new List<string>();
    //    if (publicNoticeIdsList.Any())
    //    {
    //        var publicNoticesCollection = Context.GetCollection<PublicNotice>();
    //        var publicNoticeFilter = Builders<PublicNotice>.Filter.And(
    //            Builders<PublicNotice>.Filter.In(pn => pn.Id, publicNoticeIdsList),
    //            Builders<PublicNotice>.Filter.Eq(pn => pn.IsPublished, true)
    //        );
    //        var examCategoriesCursor = await publicNoticesCollection
    //            .DistinctAsync<string>("ExamCategory", publicNoticeFilter);
    //        examCategoriesList = await examCategoriesCursor.ToListAsync();
    //    }

    //    return new FilterParameters
    //    {
    //        TypeQuestions = [.. typeQuestionsList],
    //        ExamCategories = [.. examCategoriesList],
    //        MainAreas = [.. mainAreasList],
    //        SubAreas = [.. subAreasList]
    //    };
    //}

    //public async Task<PageResult<Question>> FindQuestionsPaginatedAsync(SearchParameters searchParameter)
    //{
    //    var filterBuilder = Builders<Question>.Filter;
    //    var filters = new List<FilterDefinition<Question>>();
        
    //    if (searchParameter.IsPublished)
    //    {
    //        filters.Add(filterBuilder.Eq(q => q.IsPublished, true));
    //    }

    //    if (searchParameter.TypeQuestions is { Length: > 0 })
    //        filters.Add(filterBuilder.In(q => q.Type, searchParameter.TypeQuestions));

    //    if (searchParameter.ExamCategories is { Length: > 0 })
    //    {
    //        // Filtrar por PublicNoticeId que tenham a categoria desejada E que estejam publicados
    //        var publicNoticeBuilder = Builders<PublicNotice>.Filter;
    //        var categoryFilter = publicNoticeBuilder.And(
    //            publicNoticeBuilder.In(pn => pn.ExamCategory, searchParameter.ExamCategories),
    //            publicNoticeBuilder.Eq(pn => pn.IsPublished, true)
    //        );

    //        var publicNoticesCollection = Context.GetCollection<PublicNotice>();
    //        var publicNoticeIds = await publicNoticesCollection
    //            .Find(categoryFilter)
    //            .Project(pn => pn.Id)
    //            .ToListAsync();

    //        if (publicNoticeIds.Any())
    //            filters.Add(filterBuilder.In(q => q.PublicNoticeId, publicNoticeIds));
    //    }

    //    if (searchParameter.MainAreas is { Length: > 0 })
    //        filters.Add(filterBuilder.In(q => q.MainArea, searchParameter.MainAreas));

    //    if (searchParameter.SubAreas is { Length: > 0 })
    //        filters.Add(filterBuilder.ElemMatch(q => q.SubAreas, sa => searchParameter.SubAreas.Contains(sa)));

    //    if (!string.IsNullOrWhiteSpace(searchParameter.WordKey))
    //    {
    //        var textFilter = filterBuilder.ElemMatch(
    //            q => q.QuestionContents,
    //            Builders<ContentBlock>.Filter.OfType<ParagraphBlock>(
    //                Builders<ParagraphBlock>.Filter.ElemMatch(
    //                    p => p.Inlines,
    //                    Builders<InlineContent>.Filter.OfType<TextInline>(
    //                        Builders<TextInline>.Filter.Regex(
    //                            t => t.Text,
    //                            new BsonRegularExpression(searchParameter.WordKey, "i")
    //                        )
    //                    )
    //                )
    //            )
    //        );

    //        filters.Add(textFilter);
    //    }

    //    var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

    //    var totalItems = await DbSet.CountDocumentsAsync(finalFilter);

    //    var items = await DbSet.Find(finalFilter)
    //        .Skip((searchParameter.CurrentPage - 1) * searchParameter.PageSize)
    //        .Limit(searchParameter.PageSize)
    //        .ToListAsync();

    //    return new PageResult<Question>
    //    {
    //        Items = items,
    //        PageNumber = searchParameter.CurrentPage,
    //        PageSize = searchParameter.PageSize,
    //        TotalItems = totalItems
    //    };
    //}

    //public async Task UpdateManyAsync(List<Question> questions)
    //{
    //    var operations = questions.Select(question =>
    //        new ReplaceOneModel<Question>(
    //            Builders<Question>.Filter.Eq(q => q.Id, question.Id),
    //            question
    //        )
    //        {
    //            IsUpsert = false
    //        }
    //    ).ToList();

    //    if (operations.Count > 0)
    //    {
    //        await DbSet.BulkWriteAsync(operations);
    //    }
    //}
}
