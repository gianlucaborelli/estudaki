using MongoDB.Bson;
using MongoDB.Driver;
using ProvaOnline.Data.Context;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;
using System.Runtime.Intrinsics.X86;
using ZstdSharp.Unsafe;

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
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                throw;
            }
        }

        public async Task<QuestionDocument?> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(q => q._id == id).FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetDistinctPropertyValuesAsync(string nomePropriedade)
        {
            var valoresDistintos = await _collection
                .DistinctAsync<string>(nomePropriedade, FilterDefinition<QuestionDocument>.Empty);
            return await valoresDistintos.ToListAsync();
        }

        public async Task<FilterParameters> QueryDistinctPropertiesAsync(FilterParameters filterParameters)
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


        public async Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchFilter filter)
        {
            var filterBuilder = Builders<QuestionDocument>.Filter;
            var filters = new List<FilterDefinition<QuestionDocument>>();

            // Dynamic filters
            if (!string.IsNullOrWhiteSpace(filter.MainArea))
                filters.Add(filterBuilder.Eq(q => q.MainArea, filter.MainArea));

            if (!string.IsNullOrWhiteSpace(filter.SubArea))
                filters.Add(filterBuilder.AnyEq(q => q.SubAreas, filter.SubArea));

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                filters.Add(filterBuilder.Regex(q => q.QuestionBody, new BsonRegularExpression(filter.Keyword, "i")));

            if (!string.IsNullOrWhiteSpace(filter.ExamBoardName))
                filters.Add(filterBuilder.Eq("PublicNotice.ExamBoard", filter.ExamBoardName));

            var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

            // Total items count
            var totalItems = await _collection.CountDocumentsAsync(finalFilter);

            // Paginated items
            var items = await _collection.Find(finalFilter)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Limit(filter.PageSize)
                .ToListAsync();

            return new PageResult<QuestionDocument>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task UpdateMany(List<QuestionDocument> questions)
        {
            var operations = questions.Select(question =>
                 new ReplaceOneModel<QuestionDocument>(
                     Builders<QuestionDocument>.Filter.Eq(q => q._id, question._id),
                     question
                 )
                 {
                     IsUpsert = false // true se quiser criar caso não exista
                 }
             ).ToList();

            if (operations.Count > 0)
            {
                await _collection.BulkWriteAsync(operations);
            }
        }
    }
}
