using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    Task<PageResult<Question>> FindQuestionsPaginatedAsync(SearchParameters searchParameter);
    Task<List<Question>> GetByPublicNoticeId(string publicNoticeId);
}
