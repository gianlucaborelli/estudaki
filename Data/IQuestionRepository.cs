using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;

namespace ProvaOnline.Data
{
    public interface IQuestionRepository
    {
        Task AddAsync(QuestionDocument question);
        Task<QuestionDocument?> GetByIdAsync(string id);
        Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchFilter filter);
    }
}
