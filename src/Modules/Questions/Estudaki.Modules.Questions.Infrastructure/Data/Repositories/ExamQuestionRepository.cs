using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class ExamQuestionRepository : BaseRepository<ExamQuestion>, IExamQuestionRepository
{
    public ExamQuestionRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<List<ExamQuestion>> GetByExamId(string examId)
    {
        var filter = Builders<ExamQuestion>.Filter.Eq(eq => eq.ExamId, examId);
        return await DbSet.Find(filter).ToListAsync();
    }

    public async Task<List<ExamQuestion>> GetByQuestionId(string questionId)
    {
        var filter = Builders<ExamQuestion>.Filter.Eq(eq => eq.QuestionId, questionId);
        return await DbSet.Find(filter).ToListAsync();
    }

    public async Task<ExamQuestion?> GetByExamAndQuestion(string examId, string questionId)
    {
        var filter = Builders<ExamQuestion>.Filter.And(
            Builders<ExamQuestion>.Filter.Eq(eq => eq.ExamId, examId),
            Builders<ExamQuestion>.Filter.Eq(eq => eq.QuestionId, questionId)
        );
        return await DbSet.Find(filter).FirstOrDefaultAsync();
    }
}
