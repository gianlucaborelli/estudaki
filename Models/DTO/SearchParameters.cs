using System.Web;

namespace ProvaOnline.Models.DTO
{
    public class SearchParameters
    {
        public bool IsPublished { get; set; } = true;
        public string WordKey { get; set; } = string.Empty;
        public string[] TypeQuestions { get; set; } = [];
        public string[] MainAreas { get; set; } = [];
        public string[] SubAreas { get; set; } = [];
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public override string ToString()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["IsPublished"] = IsPublished.ToString().ToLower();
            query["WordKey"] = WordKey;
            query["CurrentPage"] = CurrentPage.ToString();
            query["PageSize"] = PageSize.ToString();

            if (TypeQuestions.Length > 0)
                query["TypeQuestions"] = string.Join(",", TypeQuestions);

            if (MainAreas.Length > 0)
                query["MainAreas"] = string.Join(",", MainAreas);

            if (SubAreas.Length > 0)
                query["SubAreas"] = string.Join(",", SubAreas);

            return query.ToString(); 
        }
    }
}
