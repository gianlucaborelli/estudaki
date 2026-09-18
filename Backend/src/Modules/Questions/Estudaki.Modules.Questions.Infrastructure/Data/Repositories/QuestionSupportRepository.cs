using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories
{
    public class QuestionSupportRepository : BaseRepository<QuestionSupport>, IQuestionSupportRepository
    {
        public QuestionSupportRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<QuestionSupport>> GetAllByPublicNoticeIdAsync(string publicNoticeId)
        {
            var filter = Builders<QuestionSupport>.Filter.Eq(qs => qs.PublicNoticeId, publicNoticeId);
            var questionSupports = await DbSet.FindAsync(filter);
            return await questionSupports.ToListAsync();
        }

        public async Task<(List<QuestionSupport> items, long totalItems)> GetByPublicNoticeIdPagedAsync
            (string publicNoticeId,
            int page,
            int pageSize,
            string? sortLabel,
            string? sortDirection)
        {
            var filter = Builders<QuestionSupport>.Filter.Eq(qs => qs.PublicNoticeId, publicNoticeId);
            var questionSupports = await DbSet.FindAsync(filter);
            var totalItems = await DbSet.CountDocumentsAsync(filter);
            var sort = GetSortDefinition<QuestionSupport>(sortLabel, sortDirection);

            page = Math.Max(page, 0);
            pageSize = Math.Max(pageSize, 1);

            var items = await DbSet
                .Find(filter)
                .Sort(sort)
                .Skip(page * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<List<QuestionSupport>> GetByIds(List<string> ids)
        {
            if (ids == null || !ids.Any())
                return new List<QuestionSupport>();

            var filter = Builders<QuestionSupport>.Filter.In(qs => qs.Id, ids);
            var questionSupports = await DbSet.FindAsync(filter);
            return await questionSupports.ToListAsync();
        }

        
    }
}
