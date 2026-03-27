using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Estudaki.Modules.Questions.Infrastructure.Data.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _collection;

    public QuestionRepository(IMongoContext context)
    {
        _collection = context.GetCollection<Question>("Questions");
    }

    public async Task AddAsync(Question question)
    {
        await _collection.InsertOneAsync(question);
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
    {
        var builder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        if (filterParameters.TypeQuestions is { Length: > 0 })
            filters.Add(builder.In(q => q.QuestionType, filterParameters.TypeQuestions));

        if (filterParameters.MainAreas is { Length: > 0 })
            filters.Add(builder.In(q => q.MainArea, filterParameters.MainAreas));

        if (filterParameters.SubAreas is { Length: > 0 })
            filters.Add(builder.In("SubAreas", filterParameters.SubAreas));

        var combinedFilter = filters.Any() ? builder.And(filters) : builder.Empty;

        var typeQuestionsTask = _collection.DistinctAsync<string>("QuestionType", combinedFilter);
        var mainAreasTask = _collection.DistinctAsync<string>("MainArea", combinedFilter);
        var subAreasTask = _collection.DistinctAsync<string>("SubAreas", combinedFilter);

        await Task.WhenAll(typeQuestionsTask, mainAreasTask, subAreasTask);

        return new FilterParameters
        {
            TypeQuestions = [.. (await typeQuestionsTask.Result.ToListAsync())],
            MainAreas = [.. (await mainAreasTask.Result.ToListAsync())],
            SubAreas = [.. (await subAreasTask.Result.ToListAsync())]
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
            filters.Add(filterBuilder.In(q => q.QuestionType, searchParameter.TypeQuestions));

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

        var totalItems = await _collection.CountDocumentsAsync(finalFilter);

        var items = await _collection.Find(finalFilter)
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
            await _collection.BulkWriteAsync(operations);
        }
    }
}
