using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Entities2;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IQuestionRepository2 : IRepository<Question2>
{
    Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    Task<PageResult<Question2>> FindQuestionsPaginatedAsync(SearchParameters searchParameter);
    Task<List<Question2>> GetByPublicNoticeId(string publicNoticeId);
}
