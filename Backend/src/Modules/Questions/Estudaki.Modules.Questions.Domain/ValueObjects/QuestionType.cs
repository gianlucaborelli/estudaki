namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class QuestionType
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
