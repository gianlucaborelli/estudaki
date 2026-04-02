namespace EstudaKi.Web.ViewModels;

public class PublicNoticeViewModel
{
    public string Id { get; init; } = string.Empty;
    public string? Number { get; init; }
    public int Year { get; init; }
    public string? ExamPhase { get; init; }
    public string? ExamBoard { get; init; }
    public string? Position { get; init; }
    public string? ExamBookletUrl { get; init; }
    public string? AnswerKeyUrl { get; init; }
    public DateTime CreatedAt { get; init; }

    // Propriedades computadas para a view
    public string DisplayName => $"{ExamBoard} - {Position} {ExamPhase} {Year}";
    public string ShortDisplayName => $"{ExamBoard} {Year}";
    public bool HasExamBooklet => !string.IsNullOrWhiteSpace(ExamBookletUrl);
    public bool HasAnswerKey => !string.IsNullOrWhiteSpace(AnswerKeyUrl);
}
