using MongoDB.Bson;
using MongoDB.Driver;
using ProvaOnline.Data.Context;
using ProvaOnline.Models;

namespace ProvaOnline.Data
{
    public class PublicNoticeRepository : IPublicNoticeRepository
    {
        private readonly IMongoCollection<PublicNoticeDocument> _collection;

        public PublicNoticeRepository(IMongoContext context)
        {
            _collection = context.GetCollection<PublicNoticeDocument>("PublicNotices");
        }

        public async Task<PublicNoticeDocument?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var filter = Builders<PublicNoticeDocument>.Filter.Eq(p => p.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<PublicNoticeDocument>> GetByIdsAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<PublicNoticeDocument>();

            var filter = Builders<PublicNoticeDocument>.Filter.In(p => p.Id, ids);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
