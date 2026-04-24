using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Questions.Domain.Entities;

/// <summary>
/// Entidade que representa um edital de concurso, que pode conter várias questões. 
/// O edital é a base para organizar as questões, e pode conter informações como número, ano, fase do exame, banca organizadora, posição (cargo), etc. 
/// Ele também pode conter URLs para o caderno de provas e gabarito oficial, que podem ser usados para referência na criação das questões. 
/// O edital é essencial para garantir que as questões estejam alinhadas com o conteúdo e formato do exame correspondente.
/// 
/// Os arquivos são armazenados seguindo a estrutura: {bucket-name}/files/exams/{year}/{examBoard}/{publicNoticeId}/
/// Os arquivos são nomeados usando o ID do edital para garantir unicidade, 
/// e podem incluir o caderno de provas (publicNoticeId.pdf) e o gabarito (publicNoticeId-answer-key.pdf).
/// 
/// </summary>
[CollectionName("public_notices")]
public class PublicNotice : Entity
{
    public string? Number { get; set; }
    public int Year { get; set; }
    public string? ExamPhase { get; set; }
    public string? ExamBoard { get; set; }

    /// <summary>
    /// Categoria do exame (armazenado como string no banco de dados)
    /// </summary>
    public string ExamCategory { get; set; } = ExamCategories.PublicServiceExam;

    public string? Position { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool HasAttachments { get; set; } = false;

    /// <summary>
    /// Indica se o edital foi revisado e está pronto para publicação
    /// </summary>
    public bool IsReviewed { get; set; } = false;

    /// <summary>
    /// Indica se a prova está publicada e visível para os usuários
    /// </summary>
    public bool IsPublished { get; set; } = false;
}

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
            BarExam => "Exame de Ordem (OAB)",
            NationalExam => "ENEM e Similares",
            SchoolExam => "Provas Escolares",
            _ => category
        };
    }
}