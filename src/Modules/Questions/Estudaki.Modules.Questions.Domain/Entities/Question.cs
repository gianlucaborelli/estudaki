using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("questions")]
public class Question : Entity
{
    public string? PublicNoticeId { get; set; }
    public List<string> QuestionSupports { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = false;
    public bool? IsNullified { get; set; } = false;
    public int QuestionNumber { get; set; }
    public string Type { get; set; } = QuestionType.MultipleChoice;
    public string MainArea { get; set; } = string.Empty;
    public string[] SubAreas { get; set; } = [];
    public List<ContentBlock> QuestionContents { get; set; } = [];
    public List<Choice>? Choices { get; set; }
}

public static class QuestionType
{
    public const string MultipleChoice = "multiple-choice";
    public const string OpenEnded = "open-ended";
    public const string Redaction = "redaction";

    public static readonly string[] All =
    {
        MultipleChoice, OpenEnded, Redaction
    };

    /// <summary>
    /// Retorna o nome amigável do Tipo da questão para exibição na interface do usuário.
    /// </summary>
    public static string GetDisplayName(string type)
    {
        return type switch
        {
            MultipleChoice => "Múltipla Escolha",
            OpenEnded => "Aberta",
            Redaction => "Redação",
            _ => "Desconhecido"
        };
    }
}
