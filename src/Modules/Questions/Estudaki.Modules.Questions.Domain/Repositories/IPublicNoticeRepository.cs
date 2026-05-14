using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IPublicNoticeRepository : IRepository<PublicNotice>
{
    Task<List<PublicNotice>> GetByIds(List<string> ids);
    Task<List<PublicNotice>> GetPublicNoticesList();
    Task<PublicNotice> GetPublicNoticeByExamId(string examId);
    Task<PublicNotice> GetByExamId(string examId);
}
