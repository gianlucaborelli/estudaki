using MongoDB.Bson;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Data
{
    public interface IQuestionRepository
    {
        Task AddAsync(QuestionDocument question);
        Task<QuestionDocument?> GetByIdAsync(string id);
        Task<List<QuestionDocument>> GetAllAsync();
        Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
        Task<PageResult<QuestionDocument>> FindQuestionsPaginatedAsync(SearchParameters searchParameter);
        Task UpdateManyAsync(List<QuestionDocument> questions);
    }
}
