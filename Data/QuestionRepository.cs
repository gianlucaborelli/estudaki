using MongoDB.Bson;
using MongoDB.Driver;
using ProvaOnline.Data.Context;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;

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

        public async Task<QuestionDocument?> GetByIdAsync(string id)
        {
            return await _collection.Find(q => q.Id == id).FirstOrDefaultAsync();
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
    }
}
