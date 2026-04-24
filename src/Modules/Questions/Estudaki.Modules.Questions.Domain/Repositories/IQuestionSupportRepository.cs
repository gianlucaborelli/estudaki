using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Repositories
{
    public interface IQuestionSupportRepository : IRepository<QuestionSupport>
    {
        /// <summary>
        /// Busca todos os QuestionSupports de um edital específico
        /// </summary>
        Task<List<QuestionSupport>> GetByPublicNoticeId(string publicNoticeId);

        /// <summary>
        /// Busca múltiplos QuestionSupports por seus IDs
        /// </summary>
        Task<List<QuestionSupport>> GetByIds(List<string> ids);
    }
}
