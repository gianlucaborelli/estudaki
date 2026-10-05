using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    Task<FilterParameters> FindFilterParametersAsync();
    Task<(List<Question> Questions, long TotalCount)> FindQuestionsPaginatedAsync(FilterParameters searchParameter);
    Task<List<Question>> GetByExamId(string examId);
    Task<(List<Question> Questions, long TotalCount)> GetByExamIdPaged(string examId, int page, int pageSize, string? sortLabel, string? sortDirection);
    Task<List<Question>> GetManyById(List<string> questionIds);
    Task<List<Question>> GetByPublicNoticeId(string publicNoticeId);
    Task<List<(string QuestionId, DateTime CreatedAt)>> GetPublishedQuestionsForSitemapAsync();
}
