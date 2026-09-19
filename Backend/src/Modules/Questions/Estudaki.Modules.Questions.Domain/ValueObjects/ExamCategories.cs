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
}
