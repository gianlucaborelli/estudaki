using ProvaOnline.Models;

namespace ProvaOnline.Data
{
    public interface IPublicNoticeRepository
    {
        Task<PublicNoticeDocument?> GetByIdAsync(string id);
        Task<List<PublicNoticeDocument>> GetByIdsAsync(List<string> ids);
    }
}
