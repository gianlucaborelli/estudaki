using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(IMongoContext context) : base(context)
    {
    }        

    public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
    {
        var builder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        if (filterParameters.TypeQuestions is { Length: > 0 })
            filters.Add(builder.In(q => q.Type, filterParameters.TypeQuestions));

        if (filterParameters.MainAreas is { Length: > 0 })
            filters.Add(builder.In(q => q.MainArea, filterParameters.MainAreas));

        if (filterParameters.SubAreas is { Length: > 0 })
            filters.Add(builder.In("SubAreas", filterParameters.SubAreas));

        var combinedFilter = filters.Any() ? builder.And(filters) : builder.Empty;

        var typeQuestionsTask = DbSet.DistinctAsync<string>("Type", combinedFilter);
        var mainAreasTask = DbSet.DistinctAsync<string>("MainArea", combinedFilter);
        var subAreasTask = DbSet.DistinctAsync<string>("SubAreas", combinedFilter);

        await Task.WhenAll(typeQuestionsTask, mainAreasTask, subAreasTask);

        var typeQuestions = await typeQuestionsTask;
        var mainAreas = await mainAreasTask;
        var subAreas = await subAreasTask;

        var typeQuestionsList = await typeQuestions.ToListAsync();
        var mainAreasList = await mainAreas.ToListAsync();
        var subAreasList = await subAreas.ToListAsync();

        return new FilterParameters
        {
            TypeQuestions = [.. typeQuestionsList],
            MainAreas = [.. mainAreasList],
            SubAreas = [.. subAreasList]
        };
    }

    public async Task<PageResult<Question>> FindQuestionsPaginatedAsync(SearchParameters searchParameter)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        if (searchParameter.IsPublished)
        {
            filters.Add(filterBuilder.Eq(q => q.IsPublished, true));
        }

        if (searchParameter.TypeQuestions is { Length: > 0 })
            filters.Add(filterBuilder.In(q => q.Type, searchParameter.TypeQuestions));

        if (searchParameter.MainAreas is { Length: > 0 })
            filters.Add(filterBuilder.In(q => q.MainArea, searchParameter.MainAreas));

        if (searchParameter.SubAreas is { Length: > 0 })
            filters.Add(filterBuilder.ElemMatch(q => q.SubAreas, sa => searchParameter.SubAreas.Contains(sa)));

        if (!string.IsNullOrWhiteSpace(searchParameter.WordKey))
        {
            var textFilter = filterBuilder.ElemMatch(
                q => q.QuestionContents,
                Builders<ContentBlock>.Filter.OfType<ParagraphBlock>(
                    Builders<ParagraphBlock>.Filter.ElemMatch(
                        p => p.Inlines,
                        Builders<InlineContent>.Filter.OfType<TextInline>(
                            Builders<TextInline>.Filter.Regex(
                                t => t.Text,
                                new BsonRegularExpression(searchParameter.WordKey, "i")
                            )
                        )
                    )
                )
            );

            filters.Add(textFilter);
        }

        var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        var totalItems = await DbSet.CountDocumentsAsync(finalFilter);

        var items = await DbSet.Find(finalFilter)
            .Skip((searchParameter.CurrentPage - 1) * searchParameter.PageSize)
            .Limit(searchParameter.PageSize)
            .ToListAsync();

        return new PageResult<Question>
        {
            Items = items,
            PageNumber = searchParameter.CurrentPage,
            PageSize = searchParameter.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task UpdateManyAsync(List<Question> questions)
    {
        var operations = questions.Select(question =>
            new ReplaceOneModel<Question>(
                Builders<Question>.Filter.Eq(q => q.Id, question.Id),
                question
            )
            {
                IsUpsert = false
            }
        ).ToList();

        if (operations.Count > 0)
        {
            await DbSet.BulkWriteAsync(operations);
        }
    }
}
