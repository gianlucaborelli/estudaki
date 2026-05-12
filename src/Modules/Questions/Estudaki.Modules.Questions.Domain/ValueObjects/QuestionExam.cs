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
    /// Cargos/Posições para os quais esta questão é aplicável (desnormalizado para facilitar filtros)
    /// Exemplo: ["Analista Judiciário", "Técnico Administrativo"]
    /// </summary>
    public List<string> Positions { get; set; } = [];
}
