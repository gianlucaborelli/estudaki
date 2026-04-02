using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace EstudaKi.Web.ViewModels;

public class QuestionViewModel
{
    public string Id { get; init; } = string.Empty;
    public int QuestionNumber { get; init; }
    public string QuestionType { get; init; } = string.Empty;
    public string MainArea { get; init; } = string.Empty;
    public string[] SubAreas { get; init; } = [];
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<ContentBlock> QuestionContents { get; init; } = [];
    public IReadOnlyList<Choice> Choices { get; init; } = [];
    public PublicNoticeViewModel? PublicNotice { get; init; }

    // Estado da view
    public bool ShowAnswers { get; set; }
    public string? SelectedAnswer { get; set; }

    // Propriedades computadas
    public bool HasChoices => Choices.Any();
    public bool HasPublicNotice => PublicNotice is not null;
    public string QuestionDetailsUrl => $"/question/{Id}";
}
