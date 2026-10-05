using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Questions.Domain.Common;

public record FilterParameters : PagedQuery
{
    public string WordKey { get; set; } = string.Empty;
    public int[] Year { get; set; } = [];
    public string[] ExaminerOrganization { get; set; } = [];
    public string[] ContractingOrganization { get; set; } = [];
    public string[] TypeQuestions { get; set; } = [];
    public string[] ExamCategories { get; set; } = [];
    public string[] MainAreas { get; set; } = [];
    public string[] SubAreas { get; set; } = [];
}
