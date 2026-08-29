namespace Estudaki.Modules.Questions.Domain.ValueObjects;

/// <summary>
/// Categorias de exames (constantes string para usar no banco de dados)
/// </summary>
public static class ExamCategories
{
    public const string UniversityEntranceExam = "UniversityEntranceExam";   // Vestibular
    public const string PublicServiceExam = "PublicServiceExam";              // Concurso público
    public const string BarExam = "BarExam";                                  // Exame de ordem (OAB)
    public const string NationalExam = "NationalExam";                        // ENEM e similares
    public const string SchoolExam = "SchoolExam";                            // Provas escolares

    /// <summary>
    /// Retorna todos os valores possíveis de categorias
    /// </summary>
    public static readonly string[] All =
    {
    UniversityEntranceExam,
    PublicServiceExam,
    BarExam,
    NationalExam,
    SchoolExam
};

    /// <summary>
    /// Retorna o nome amigável da categoria
    /// </summary>
    public static string GetDisplayName(string category)
    {
        return category switch
        {
            UniversityEntranceExam => "Vestibular",
            PublicServiceExam => "Concurso Público",
            BarExam => "Exame de Ordem",
            NationalExam => "ENEM e Similares",
            SchoolExam => "Provas Escolares",
            _ => category
        };
    }
}
