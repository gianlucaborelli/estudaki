using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Comunications.Domain.Entities;
using Estudaki.Modules.Comunications.Domain.Repositories;

namespace Estudaki.Modules.Comunications.Infrastructure.Data.Repositories;

public class ContactMessageRepository : BaseRepository<ContactMessage>, IContactMessageRepository
{
    public ContactMessageRepository(IMongoContext context) : base(context)
    {
    }
}
