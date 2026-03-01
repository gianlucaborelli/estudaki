using MongoDB.Bson;
using MongoDB.Driver;
using ProvaOnline.Data.Context;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Data
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IMongoCollection<QuestionDocument> _collection;

        public QuestionRepository(IMongoContext context)
        {
            _collection = context.GetCollection<QuestionDocument>("Questions");
        }

        public async Task AddAsync(QuestionDocument question)
        {
            await _collection.InsertOneAsync(question);
        }

        public async Task<List<QuestionDocument>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<QuestionDocument?> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(q => q._id == id).FirstOrDefaultAsync();
        }

        public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
        {
            var builder = Builders<QuestionDocument>.Filter;
            var filters = new List<FilterDefinition<QuestionDocument>>();

            if (filterParameters.TypeQuestions is { Length: > 0 })
                filters.Add(builder.In("QuestionType", filterParameters.TypeQuestions));

            if (filterParameters.MainAreas is { Length: > 0 })
                filters.Add(builder.In("MainArea", filterParameters.MainAreas));

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

        public async Task<PageResult<QuestionDocument>> FindQuestionsPaginatedAsync(SearchParameters searchParameter)
        {
            var filterBuilder = Builders<QuestionDocument>.Filter;
            var filters = new List<FilterDefinition<QuestionDocument>>();

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
                filters.Add(filterBuilder.Regex(q => q.QuestionBody, new BsonRegularExpression(searchParameter.WordKey, "i")));

            var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

            var totalItems = await _collection.CountDocumentsAsync(finalFilter);

            var items = await _collection.Find(finalFilter)
                .Skip((searchParameter.CurrentPage - 1) * searchParameter.PageSize)
                .Limit(searchParameter.PageSize)
                .ToListAsync();

            return new PageResult<QuestionDocument>
            {
                Items = items,
                PageNumber = searchParameter.CurrentPage,
                PageSize = searchParameter.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task UpdateManyAsync(List<QuestionDocument> questions)
        {
            var operations = questions.Select(question =>
                 new ReplaceOneModel<QuestionDocument>(
                     Builders<QuestionDocument>.Filter.Eq(q => q._id, question._id),
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
}
