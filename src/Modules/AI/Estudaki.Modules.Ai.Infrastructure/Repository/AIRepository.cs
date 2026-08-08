using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Ai.Application.Interfaces;
using Estudaki.Modules.Ai.Domain.Entities;
using MongoDB.Driver;

namespace Estudaki.Modules.Ai.Infrastructure.Repository;

public class AIRepository : BaseRepository<AIPrompt>, IAiRepository
{
    public AIRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<AIPrompt?> GetByNameAsync(string name)
    {
        var filter = Builders<AIPrompt>.Filter.And(
            Builders<AIPrompt>.Filter.Eq(x => x.Name, name),
            Builders<AIPrompt>.Filter.Eq(x => x.IsActive, true));

        var result = await DbSet.FindAsync(filter);
        return await result.FirstOrDefaultAsync();
    }
}
