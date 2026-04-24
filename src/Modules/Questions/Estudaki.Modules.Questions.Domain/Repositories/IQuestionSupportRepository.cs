using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estudaki.Modules.Questions.Domain.Repositories
{
    public interface IQuestionSupportRepository : IRepository<QuestionSupport>
    {
        /// <summary>
        /// Busca todos os QuestionSupports de um edital específico
        /// </summary>
        Task<List<QuestionSupport>> GetByPublicNoticeId(string publicNoticeId);
    }
}
