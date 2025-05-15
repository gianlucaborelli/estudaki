namespace ProvaOnline.Models.DTO
{
    public class SearchParameters
    {
        public string WordKey { get; set; } = string.Empty;
        public string[] TypeQuestions { get; set; } = [];
        public string[] MainAreas { get; set; } = [];
        public string[] SubAreas { get; set; } = [];
    }
}
