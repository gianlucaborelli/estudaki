namespace Estudaki.Modules.Questions.Domain.Common;

public class SearchParameters
{
    public bool IsPublished { get; set; } = true;
    public string WordKey { get; set; } = string.Empty;
    public string[] TypeQuestions { get; set; } = [];
    public string[] ExamCategories { get; set; } = [];
    public string[] MainAreas { get; set; } = [];
    public string[] SubAreas { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
