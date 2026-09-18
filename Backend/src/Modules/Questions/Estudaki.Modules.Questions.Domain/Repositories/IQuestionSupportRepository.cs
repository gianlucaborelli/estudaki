using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories
{
    public interface IQuestionSupportRepository : IRepository<QuestionSupport>
    {
        Task<List<QuestionSupport>> GetAllByPublicNoticeIdAsync(string publicNoticeId);
        Task<(List<QuestionSupport> items, long totalItems)> GetByPublicNoticeIdPagedAsync(
            string publicNoticeId,
            int page,
            int pageSize,            
            string? sortLabel,
            string? sortDirection);

        Task<List<QuestionSupport>> GetByIds(List<string> ids);
    }
}
