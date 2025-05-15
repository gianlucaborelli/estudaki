using MongoDB.Bson;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;
using ProvaOnline.Services;

namespace ProvaOnline.Data
{
    public interface IQuestionRepository
    {
        Task AddAsync(QuestionDocument question);
        Task<List<QuestionDocument>> GetAllAsync();
        Task<FilterParameters> QueryDistinctPropertiesAsync(FilterParameters filterParameters);
        Task UpdateMany(List<QuestionDocument> question);
        Task<QuestionDocument?> GetByIdAsync(ObjectId id);
        Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchService searchService);
    }
}
