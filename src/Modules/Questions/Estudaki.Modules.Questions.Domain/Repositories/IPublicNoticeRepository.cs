using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IPublicNoticeRepository
{
    Task<PublicNotice?> GetByIdAsync(string id);
    Task<List<PublicNotice>> GetByIdsAsync(List<string> ids);
}
