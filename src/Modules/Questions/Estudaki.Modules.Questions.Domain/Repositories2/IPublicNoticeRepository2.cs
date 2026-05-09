using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IPublicNoticeRepository2 : IRepository<PublicNotice>
{
    Task<List<PublicNotice>> GetByIds(List<string> ids);
    Task<List<PublicNotice>> GetPublicNoticesList();
}
