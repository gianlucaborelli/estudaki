using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Services
{
    public interface IQuestionServices
    {
        Task<QuestionWithNoticeDto?> GetQuestionByIdAsync(string id);
        Task<PageResult<QuestionWithNoticeDto>> SearchQuestionsPaginatedAsync(SearchParameters searchParameter);
        Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    }
}
