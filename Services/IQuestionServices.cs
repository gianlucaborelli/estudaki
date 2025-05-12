using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;

namespace ProvaOnline.Services
{
    public interface IQuestionServices
    {
        Task<QuestionDocument?> GetQuestionById(string id);
        Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchFilter testId);
        Task<FilterParameters> LoadFilterParameters(FilterParameters filterParameters);
    }
}
