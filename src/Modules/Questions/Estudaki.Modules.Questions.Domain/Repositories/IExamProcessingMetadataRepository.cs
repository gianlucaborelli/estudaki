using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estudaki.Modules.Questions.Domain.Repositories
{
    public interface IExamProcessingMetadataRepository : IRepository<ExamProcessingMetadata>
    {
        /// <summary>
        /// Busca todos os metadados de processamento de um edital específico
        /// </summary>
        Task<List<ExamProcessingMetadata>> GetByPublicNoticeId(string publicNoticeId);

        /// <summary>
        /// Busca metadados por ProvaId
        /// </summary>
        Task<ExamProcessingMetadata?> GetByProvaId(string provaId);

        /// <summary>
        /// Remove todos os metadados de um edital específico
        /// </summary>
        Task DeleteByPublicNoticeId(string publicNoticeId);
    }
}
