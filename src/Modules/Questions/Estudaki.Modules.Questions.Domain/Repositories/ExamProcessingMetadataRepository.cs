using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public class ExamProcessingMetadataRepository : BaseRepository<ExamProcessingMetadata>, IExamProcessingMetadataRepository
{
    public ExamProcessingMetadataRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<List<ExamProcessingMetadata>> GetByPublicNoticeId(string publicNoticeId)
    {
        var filter = Builders<ExamProcessingMetadata>.Filter.Eq(x => x.PublicNoticeId, publicNoticeId);
        var result = await DbSet.FindAsync(filter);
        return await result.ToListAsync();
    }

    public async Task<ExamProcessingMetadata?> GetByProvaId(string provaId)
    {
        var filter = Builders<ExamProcessingMetadata>.Filter.Eq(x => x.ProvaId, provaId);
        var result = await DbSet.FindAsync(filter);
        return await result.FirstOrDefaultAsync();
    }

    public async Task DeleteByPublicNoticeId(string publicNoticeId)
    {
        var filter = Builders<ExamProcessingMetadata>.Filter.Eq(x => x.PublicNoticeId, publicNoticeId);
        await DbSet.DeleteManyAsync(filter);
    }
}
