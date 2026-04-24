using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories
{
    public class QuestionSupportRepository : BaseRepository<QuestionSupport>, IQuestionSupportRepository
    {
        public QuestionSupportRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<QuestionSupport>> GetByPublicNoticeId(string publicNoticeId)
        {
            var filter = Builders<QuestionSupport>.Filter.Eq(qs => qs.PublicNoticeId, publicNoticeId);
            var questionSupports = await DbSet.FindAsync(filter);
            return await questionSupports.ToListAsync();
        }
    }
}
