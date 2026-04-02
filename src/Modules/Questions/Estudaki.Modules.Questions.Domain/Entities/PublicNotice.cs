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
    public string? Position { get; set; }    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool HasAttachments { get; set; } = false;
}
