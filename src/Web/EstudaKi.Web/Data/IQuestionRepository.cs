using MongoDB.Bson;
using EstudaKi.Web.Helper;
using EstudaKi.Web.Helpers;
using EstudaKi.Web.Models;
using EstudaKi.Web.Models.DTO;

namespace EstudaKi.Web.Data
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
