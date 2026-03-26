using EstudaKi.Web.Helper;
using EstudaKi.Web.Helpers;
using EstudaKi.Web.Models.DTO;

namespace EstudaKi.Web.Services
{
    public interface IQuestionServices
    {
        Task<QuestionWithNoticeDto?> GetQuestionByIdAsync(string id);
        Task<PageResult<QuestionWithNoticeDto>> SearchQuestionsPaginatedAsync(SearchParameters searchParameter);
        Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    }
}
