using EstudaKi.Web.Models.DTO;

namespace EstudaKi.Web.Helpers
{
    public class SearchFilter
    {
        public bool IsPublished { get; set; } = true;
        public SearchParameters SearchParameters { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
