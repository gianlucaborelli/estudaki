using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    Task<(List<Question> Questions, long TotalCount)> FindQuestionsPaginatedAsync(SearchParameters searchParameter);
    Task<List<Question>> GetByExamId(string examId);
}
