using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Infrastructure.Data.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class PublicNoticeRepository : IPublicNoticeRepository
{
    private readonly IMongoCollection<PublicNotice> _collection;

    public PublicNoticeRepository(IMongoContext context)
    {
        _collection = context.GetCollection<PublicNotice>("PublicNotices");
    }

    public async Task<PublicNotice?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var filter = Builders<PublicNotice>.Filter.Eq(p => p.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<PublicNotice>> GetByIdsAsync(List<string> ids)
    {
        if (ids == null || ids.Count == 0)
            return [];

        var filter = Builders<PublicNotice>.Filter.In(p => p.Id, ids);
        return await _collection.Find(filter).ToListAsync();
    }
}
