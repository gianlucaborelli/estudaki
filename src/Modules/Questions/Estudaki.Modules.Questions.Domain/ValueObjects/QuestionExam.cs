using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.ValueObjects;

/// <summary>
/// Representa as informações de um exame associado a uma questão.
/// Armazena dados desnormalizados para otimizar queries.
/// </summary>
public class QuestionExam
{
    /// <summary>
    /// ID do exame (documento filho dentro de PublicNotice)
    /// </summary>
    public string ExamId { get; set; } = default!;

    /// <summary>
    /// ID do edital ao qual o exame pertence
    /// </summary>
    public string PublicNoticeId { get; set; } = default!;

    /// <summary>
    /// ID do exame de onde esta questão foi originalmente extraída/criada.
    /// Útil para rastrear a fonte original da questão quando ela aparece em múltiplos exames.
    /// </summary>
    public string SourceExamId { get; set; } = default!;

    /// <summary>
    /// Número da questão no exame
    /// </summary>
    public int QuestionNumber { get; set; }

    /// <summary>
    /// Ano do exame (desnormalizado para facilitar filtros)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Categoria do exame (desnormalizado para facilitar filtros)
    /// </summary>
    public string ExamCategory { get; set; } = string.Empty;

    /// <summary>
    /// Banca examinadora (desnormalizado para facilitar filtros)
    /// </summary>
    public string? ExaminerOrganization { get; set; }

    /// <summary>
    /// Contratante do concurso ou exame (desnormalizado para facilitar filtros)
    /// Exemplo: Prefeitura de São Paulo, OAB, etc.
    /// </summary>
    public string? ContractingOrganization { get; set; }

    /// <summary>
    /// Cargo/Posição para o qual esta questão é aplicável (desnormalizado para facilitar filtros)
    /// Exemplo: "Analista Judiciário", "Técnico Administrativo"
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Fase do exame (ex: "1ª Fase", "2ª Fase", "Objetiva", "Discursiva")
    /// </summary>
    public string? Phase { get; set; }

    /// <summary>
    /// Área de conhecimento do exame (ex: "Direito", "Tecnologia da Informação")
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Nível de escolaridade exigido (ex: "Superior", "Médio", "Fundamental")
    /// </summary>
    public string? EducationLevel { get; set; }

    /// <summary>
    /// URL do caderno de questões do exame
    /// </summary>
    public string? ExamBookletUrl { get; set; }

    /// <summary>
    /// URL do gabarito oficial do exame
    /// </summary>
    public string? AnswerKeyUrl { get; set; }

    public static QuestionExam Create(Exam exam, PublicNotice publicNotice)
    {
        return new QuestionExam
        {
            ExamId = exam.Id,
            PublicNoticeId = publicNotice.Id,
            SourceExamId = exam.Id,
            QuestionNumber = 0, // Inicialmente 0, será definido quando a questão for adicionada ao exame
            Year = publicNotice.Year,
            ExamCategory = publicNotice.ExamCategory,
            ExaminerOrganization = publicNotice.ExaminerOrganization,
            ContractingOrganization = publicNotice.ContractingOrganization,
            EducationLevel = exam.EducationLevel,
            Position = exam.Position,
            Phase = exam.Phase,
            Area = exam.Area,            
            ExamBookletUrl = exam.ExamBookletUrl,
            AnswerKeyUrl = exam.AnswerKeyUrl
        };
    }
}
