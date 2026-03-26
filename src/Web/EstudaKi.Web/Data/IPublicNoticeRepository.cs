using EstudaKi.Web.Models;

namespace EstudaKi.Web.Data
{
    public interface IPublicNoticeRepository
    {
        Task<PublicNoticeDocument?> GetByIdAsync(string id);
        Task<List<PublicNoticeDocument>> GetByIdsAsync(List<string> ids);
    }
}
