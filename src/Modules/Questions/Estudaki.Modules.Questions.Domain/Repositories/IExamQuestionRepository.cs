using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IExamQuestionRepository : IRepository<ExamQuestion>
{
    Task<List<ExamQuestion>> GetByExamId(string examId);
    Task<List<ExamQuestion>> GetByQuestionId(string questionId);
    Task<ExamQuestion?> GetByExamAndQuestion(string examId, string questionId);
}
