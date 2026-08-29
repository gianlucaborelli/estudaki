namespace EstudaKi.Web.Helpers;

/// <summary>
/// Helper para trabalhar com tipos de questão de forma amigável ao usuário
/// </summary>
public static class QuestionTypeHelper
{
    public const string MultipleChoice = "multiple-choice";
    public const string OpenEnded = "open-ended";
    public const string Redaction = "redaction";

    /// <summary>
    /// Converte o tipo de questão técnico para um nome amigável em português
    /// </summary>
    /// <param name="type">Tipo técnico da questão (ex: "multiple-choice")</param>
    /// <returns>Nome amigável em português (ex: "Múltipla Escolha")</returns>
    public static string GetDisplayName(string type)
    {
        return type switch
        {
            MultipleChoice => "Múltipla Escolha",
            OpenEnded => "Aberta",
            Redaction => "Redação",
            _ => type // Retorna o próprio tipo se não for reconhecido
        };
    }

    /// <summary>
    /// Converte um array de tipos técnicos para uma lista de tuplas (valor, displayName)
    /// Útil para popular dropdowns e selects
    /// </summary>
    public static List<(string Value, string DisplayName)> GetDisplayList(string[] types)
    {
        return types
            .Select(t => (Value: t, DisplayName: GetDisplayName(t)))
            .OrderBy(t => t.DisplayName)
            .ToList();
    }

    /// <summary>
    /// Converte um dicionário de tipos para exibição em componentes MudBlazor
    /// </summary>
    public static Dictionary<string, string> GetDisplayDictionary(string[] types)
    {
        return types.ToDictionary(t => t, t => GetDisplayName(t));
    }
}
