using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class PublicNoticeRepository : BaseRepository<PublicNotice>, IPublicNoticeRepository
{
    public PublicNoticeRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<List<PublicNotice>> GetPublicNoticesList()
    {
        return await DbSet.Find(_ => true).ToListAsync();
    }

    public async Task<List<PublicNotice>> GetByIds(List<string> ids)
    {
        if (ids == null || ids.Count == 0)
            return [];

        var filter = Builders<PublicNotice>.Filter.In(p => p.Id, ids);
        return await DbSet.Find(filter).ToListAsync();
    }
}
