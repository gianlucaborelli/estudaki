using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;

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

    /// <summary>
    /// Ano do concurso ou exame.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Banca examinadora responsável pelo concurso ou exame. Exemplo: FGV, Vunesp, Cespe, etc.
    /// </summary>
    public string? ExaminerOrganization { get; set; }

    /// <summary>
    /// Contratante do concurso ou exame. Exemplo: Prefeitura de São Paulo, OAB, etc.
    /// </summary>
    public string? ContractingOrganization { get; set; }

    /// <summary>
    /// Coleção de exames associados a este edital. Cada exame pode representar uma fase ou um cargo específico dentro do concurso.
    /// </summary>
    public List<Exam> Exams { get; set; } = [];

    /// <summary>
    /// Categoria do exame (armazenado como string no banco de dados)
    /// </summary>
    public string ExamCategory { get; set; } = ExamCategories.PublicServiceExam;        

    /// <summary>
    /// Indica se o edital foi revisado e está pronto para publicação
    /// </summary>
    public bool IsReviewed { get; set; } = false;

    /// <summary>
    /// Indica se a prova está publicada e visível para os usuários
    /// </summary>
    public bool IsPublished { get; set; } = false;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
