using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Services
{
    public interface IQuestionServices
    {
        Task<QuestionDocument?> GetQuestionById(string id);
        Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchParameters searchParameter);
        Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    }
}
