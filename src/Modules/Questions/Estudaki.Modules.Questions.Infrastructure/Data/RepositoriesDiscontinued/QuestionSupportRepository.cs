using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories
{
    public class QuestionSupportRepositoryDiscontinued //: BaseRepository<QuestionSupport>, IQuestionSupportRepository
    {
        //public QuestionSupportRepositoryDiscontinued(IMongoContext context) : base(context)
        //{
        //}

        //public async Task<List<QuestionSupport>> GetByPublicNoticeId(string publicNoticeId)
        //{
        //    var filter = Builders<QuestionSupport>.Filter.Eq(qs => qs.PublicNoticeId, publicNoticeId);
        //    var questionSupports = await DbSet.FindAsync(filter);
        //    return await questionSupports.ToListAsync();
        //}

        //public async Task<List<QuestionSupport>> GetByIds(List<string> ids)
        //{
        //    if (ids == null || !ids.Any())
        //        return new List<QuestionSupport>();

        //    var filter = Builders<QuestionSupport>.Filter.In(qs => qs.Id, ids);
        //    var questionSupports = await DbSet.FindAsync(filter);
        //    return await questionSupports.ToListAsync();
        //}
    }
}
