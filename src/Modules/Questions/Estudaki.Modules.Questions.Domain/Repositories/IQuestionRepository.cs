using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IQuestionRepository
{
    Task AddAsync(Question question);
    Task<Question?> GetByIdAsync(string id);
    Task<List<Question>> GetAllAsync();
    Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters);
    Task<PageResult<Question>> FindQuestionsPaginatedAsync(SearchParameters searchParameter);
    Task UpdateManyAsync(List<Question> questions);
}
