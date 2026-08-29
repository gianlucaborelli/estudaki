namespace EstudaKi.Web.Helpers;

/// <summary>
/// Helper para trabalhar com categorias de exame de forma amigável ao usuário
/// </summary>
public static class ExamCategoryHelper
{
    public const string UniversityEntranceExam = "UniversityEntranceExam";
    public const string PublicServiceExam = "PublicServiceExam";
    public const string BarExam = "BarExam";
    public const string NationalExam = "NationalExam";
    public const string SchoolExam = "SchoolExam";

    /// <summary>
    /// Converte a categoria de exame técnica para um nome amigável em português
    /// </summary>
    /// <param name="category">Categoria técnica do exame (ex: "UniversityEntranceExam")</param>
    /// <returns>Nome amigável em português (ex: "Vestibular")</returns>
    public static string GetDisplayName(string category)
    {
        return category switch
        {
            UniversityEntranceExam => "Vestibular",
            PublicServiceExam => "Concurso Público",
            BarExam => "Exame de Ordem (OAB)",
            NationalExam => "ENEM e Similares",
            SchoolExam => "Provas Escolares",
            _ => category // Retorna a própria categoria se não for reconhecida
        };
    }

    /// <summary>
    /// Converte um array de categorias técnicas para uma lista de tuplas (valor, displayName)
    /// Útil para popular dropdowns e selects
    /// </summary>
    public static List<(string Value, string DisplayName)> GetDisplayList(string[] categories)
    {
        return categories
            .Select(c => (Value: c, DisplayName: GetDisplayName(c)))
            .OrderBy(c => c.DisplayName)
            .ToList();
    }

    /// <summary>
    /// Converte um dicionário de categorias para exibição em componentes MudBlazor
    /// </summary>
    public static Dictionary<string, string> GetDisplayDictionary(string[] categories)
    {
        return categories.ToDictionary(c => c, c => GetDisplayName(c));
    }
}
