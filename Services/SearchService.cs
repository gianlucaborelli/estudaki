using ProvaOnline.Models.DTO;

namespace ProvaOnline.Services
{
    public class SearchService
    {
        public bool IsPublished { get; set; } = true;
        public SearchParameters SearchParameters { get; set; } = new SearchParameters();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
